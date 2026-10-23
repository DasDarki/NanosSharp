# Architecture

NanosSharp adds a .NET runtime to [nanos world](https://nanos-world.com) so packages can be written in C# next to or
instead of Lua, on the dedicated server and in the game client. This document describes how the pieces fit together.
For the security model and trust boundaries see [security.md](security.md); for the history of the client sandbox work
see [wasm-migration.md](wasm-migration.md).

## The pieces

```
                         nanos world engine (native, Lua)
                                    |
                    reflection seam (IClass / IFunction / ScriptValue)
                                    |
   native host  (C++)   version.dll (client)  /  libnanossharp.so (server)
                                    |
                            hostfxr -> CoreCLR
                                    |
   trusted host (C#)         NanosSharp.Host
                            /                    \
                       server                    client
                  packages in-process        packages in a WebAssembly sandbox
```

Three layers, each with a clear job:

1. **The native host** (`native/`, C++) injects into the engine process, finds the scripting engine and hosts a .NET
   runtime through hostfxr. On the server it is `libnanossharp.so`, loaded with `LD_PRELOAD`. On the client it is
   `version.dll`, a proxy of the system `version.dll` that the game loads on start. It hooks the engine lifecycle
   (attach, tick, package load and unload, environment creation) and exposes the engine to managed code through an
   engine-agnostic reflection layer rather than the raw Lua stack.

2. **The trusted host** (`managed/NanosSharp.Host`, C#, CoreCLR) is the entry point the native host calls through
   `Entry.Initialize`. It compiles packages with Roslyn, loads them, runs their lifecycle and bridges them to the
   engine. It carries Roslyn, NuGet and the configuration parsers. The same host binary runs on both sides, built in a
   server flavor and a client flavor.

3. **The API assembly** (`managed/NanosSharp`, C#) is what package code references. It is generated per side (server,
   client, shared) from a native dump of the engine combined with the public nanos world API, so the surface always
   matches the engine it runs against.

## The interop seam

All traffic between managed code and the engine goes through a single, explicit seam, so there is exactly one place
that crosses the boundary:

- `NativeApi` is a struct of function pointers for the host-to-engine direction (create values, call functions, read
  properties, spawn entities, log, and so on). Managed code calls the engine only through `NativeRuntime.Api`.
- `ManagedCallbacks` is the reverse: a struct of function pointers the engine calls for engine-to-managed events (tick,
  engine attach and detach, a subscribed function firing, an entity being created or destroyed). `NativeCallbacks`
  fills it in.

Values and entities never cross as raw pointers that managed code dereferences. They cross as opaque handles and as a
blittable `NativeArgument` record, so the same seam works whether the managed side sits in the same process (server) or
behind a sandbox boundary (client).

## The API assembly

The generator (`xmake generate`) merges a native RTTI dump of the engine with the nanos world API JSON and writes a
deterministic, checked-in binding. One source tree builds three assemblies through the `NanosSharpSide` property:

- **Server** defines `SERVER`, exposes the server API.
- **Client** defines `CLIENT`, exposes the client API.
- **Shared** is the common subset, referenced by a package's `Shared/` code, which also sees `SERVER` or `CLIENT` for
  side specific branches.

The API is dual: Lua-near methods (`GetHealth` / `SetHealth`) plus C# properties and events that delegate to them, so
code reads like idiomatic C# while mapping one to one onto the engine.

## Package model

A C# package is a regular nanos world package with its own `Package.toml`, scripting environment and entry in
`Server.GetPackages()`. Source lives in side folders:

```
Packages/my-gamemode/
 ├─ Server/Gamemode.cs      runs on the server
 ├─ Client/Hud.cs           runs on every player with a NanosSharp client
 ├─ Client/Index.lua        Lua fallback for players without NanosSharp
 └─ Shared/Teams.cs         runs on both sides
```

The runtime discovers `Package` subclasses and runs their `OnStart` / `OnStop`, resolves `[Inject]` services through a
container, registers `[EntityFactory]` types and `[Export]` members, and wires Bukkit-style typed events. The server
compiles `*.cs` with Roslyn (or loads prebuilt `*.dll`), supports NuGet `PackageReference`, and hot reloads a package
when its sources change.

## Server execution

On the server each package is loaded into its own collectible `AssemblyLoadContext` and its own scripting environment,
compiled in memory with symbols, and run in process on CoreCLR with full trust. The server operator controls which
packages run, exactly as with Lua. This path is unchanged by the client sandbox work.

## Client execution: the WebAssembly sandbox

Client packages are pushed by the server to every player, so they are untrusted. Source is compiled with Roslyn on the
player's machine, outside the sandbox, by the trusted host; a package may instead ship prebuilt `*.dll` assemblies in its
`Client/` folder, which skip compilation. Either way the package runs inside a WebAssembly sandbox with no ambient
authority, so a prebuilt assembly is no more trusted than compiled source. The sandbox is the reason the interop seam is
handle based.

```
 trusted host (CoreCLR, client flavor)
  ├─ compiles Client/ + Shared/ sources with Roslyn, or takes a package's prebuilt Client/ assemblies as is
  ├─ stages the compiled assemblies into a per-package directory the reactor reads
  ├─ boots ONE reactor for the session and drives it each frame
  ├─ WasmHost: embeds Wasmtime, implements the `nanos` imports, owns the handle tables
  └─ relays engine callbacks into the reactor
         │  (the `nanos` import/export boundary)
 reactor (Wasmtime, Mono)
  ├─ the slim in-sandbox runtime (NanosSharp.WasmRuntime), no Roslyn, no NuGet
  ├─ loads staged package assemblies into collectible contexts and runs their lifecycle
  └─ reaches the engine only by calling the brokered `nanos` imports
```

- **The reactor** is the slim client runtime (`managed/NanosSharp.WasmRuntime`) published to `wasi-wasm` and relinked
  into a WASI reactor core module (`tools/build-reactor.sh`). It runs on the Mono interpreter. It is a reactor, not a
  command module, so the host can call its exports repeatedly after boot (`nanos_boot`, `nanos_sync_packages`,
  `nanos_tick`, `nanos_engine_attached`, `nanos_invoke_function`, the entity callbacks).

- **The seam over imports.** Inside the reactor the `NativeApi` struct is backed by direct `nanos` wasm imports instead
  of in-process function pointers, so every call site in the API assembly stays the same. `WasmHost` on the trusted
  side implements each import: it reads arguments out of the guest's linear memory, translates the 32-byte guest
  `NativeArgument` records to and from the native ones, resolves handles, calls the real engine function, and writes
  the result back. Engine values, functions, properties, environments and entities live in handle tables on the
  trusted side and cross to the guest only as small integer indices.

- **The callback relay.** When a client package subscribes to an engine event, the engine holds a function it invokes.
  In the sandbox that function is a reactor export the trusted host calls: a subscribed function fires, the engine calls
  `ManagedCallbacks`, the trusted host writes the arguments into guest memory and calls `nanos_invoke_function`, the
  reactor dispatches to the managed delegate and returns the result back across the boundary. Guest-owned function ids
  are tagged so the trusted host can tell its own callbacks (its tick) from the sandbox's and route each correctly.

- **Lifecycle.** The trusted host stages a package (writes its assemblies plus a generation marker), boots the reactor
  once, and reconciles the loaded set against the staged set each time it changes, loading, unloading and reloading
  packages without a reboot. The reactor is ticked once per frame by the host through `nanos_tick`, which pumps the
  guest's timers, main-thread queue and per-frame handlers.

Client packages therefore cannot use NuGet (there is no compiler or dependency resolver in the sandbox) and have no hot
reload. Everything else, reflection, dependency injection, events, entity factories, async, works the same as on the
server.

## Delivery

Releases are built by `xmake bundle` (server), `xmake client` (client, including the reactor) and `xmake installer`,
and published as signed artifacts. The installer verifies every download against a checked-in public key before it
writes anything. See [security.md](security.md) for the integrity model.
