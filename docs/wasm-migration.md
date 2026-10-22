# Client runtime WASM migration

Status: exploration, branch `refactor/wasm`.

## Why

The client side runs package code on every connected player's machine. Today
the only barrier is a compile-time CIL allow list (`SandboxVerifier` plus
`client-allowlist.txt`). That is a static check, not a runtime boundary: once an
assembly is loaded it runs with the full rights of the host process. .NET has no
partial-trust model to fall back on, and the supported client deployment is a
Windows build running under Proton, where OS-level sandboxing (seccomp, Sandbox2,
AppContainer) is not reliably available. So isolation must come from the
execution model itself.

A WebAssembly sandbox provides exactly that, platform-independently and
identically under Proton, because the boundary is the module edge rather than
anything the OS supplies. This matches the isolation Lua already gives the client
today (a memory-safe interpreter that can only reach what the host binds).

## Decision

Run the client packages with **the .NET runtime itself compiled to WebAssembly**
(the Mono `dotnet.wasm` model, as used by Blazor WebAssembly), embedded in the
native client through a WASM runtime, rather than AOT-compiling each package to
WASM directly.

Rationale: the package API is deeply reflection- and dynamic-loading-based
(attribute scanning for `[EventHook]`/`[ScriptClass]`/`[EntityFactory]`, the DI
container, collectible `AssemblyLoadContext` for hot reload). Direct per-package
AOT (NativeAOT-LLVM) would break all of that. Running the full runtime inside the
sandbox preserves it, so the package-facing API needs no cuts.

The runtime-in-wasm approach is validated: see `experiments/wasm-probe`.
Reflection, attribute scanning and constructor-injection DI all run correctly
inside a WASI sandbox under wasmtime.

## What the probe established (2026-10-21)

- Real .NET with reflection + DI executes inside a WASI sandbox under a real
  standalone host (wasmtime 29, .NET 10.0.303).
- The default WASI output is the **Mono interpreter**: `dotnet.wasm` (~12 MB)
  plus ~200 sidecar BCL assemblies loaded from a preopened directory. This is
  the footprint/startup cost to weigh against Lua. AOT is a separate later step
  and is what makes the performance case.
- **AOT works end-to-end and resolves the central tension.** Building the probe
  with `RunAOTCompilation=true` runs the custom-import component correctly,
  lifts the interpreter trampoline limit (the `i64` `function-call` signature
  now works), and nearly halves the module (32 MB -> 17 MB). AOT requires
  `PublishTrimmed=true`, and trimming would strip the types reflection/DI
  discover - but rooting the package assembly (`<TrimmerRootAssembly>`) keeps
  them, and reflection + DI still worked. The trimmer emits IL2026/IL2070/
  IL2072/IL2075 warnings on the reflection paths: the production constraint is
  that the runtime's attribute scanning and DI must be made trim-safe (root the
  package assemblies and/or annotate with `[DynamicallyAccessedMembers]` /
  `[DynamicDependency]`). So full C# (reflection/DI) and AOT performance do
  coexist, at the cost of trim-rooting.
- Modern .NET WASI output is a **WASI 0.2 component**, not a core module
  (`WasiApp.targets` always links `--component-type`). Consequences below.
- The baked-in component world is fixed to WASI (`wasi:cli/run` export, `wasi:*`
  imports incl. `wasi:http`); confirmed with `wasm-tools component wit`. The
  stock SDK has **no support for custom WIT imports/exports** - the only
  component-type WIT it links is the runtime's own `WasiHttpWorld`.
- Adding a single custom import (`[WasmImportLinkage][DllImport("nanos")]`)
  forces a **native relink and therefore the full wasi-sdk** (clang), set via
  `$(WASI_SDK_PATH)`. The `wasi-experimental` workload alone is not enough; the
  stock no-custom-import build succeeds only because it reuses the prebuilt
  `dotnet.wasm` with no native link step.
- In interpreter mode, custom import signatures are limited to Mono's
  precompiled interp-to-native trampoline set. A `function-call` import with
  signature `(i32, i32, i32) -> i64` traps at runtime
  (`aot-runtime-wasm.c:188 <disabled>`); the same import returning `i32` works.
  Since the real `NativeApi` has ~53 functions with varied signatures, **AOT is
  effectively required** (it generates native trampolines for every signature),
  not just a performance option. The AOT cross pack
  (`Microsoft.NETCore.App.Runtime.AOT.linux-x64.Cross.wasi-wasm`) is installed.

## The boundary (what actually has to move)

The entire managed<->native ABI is two tables of function pointers, not classic
`[DllImport]`. This maps cleanly onto a WASM import/export table (or, given the
component finding, a WIT world).

### Imports: engine functions the managed side calls
`NanosSharp/Interop/NativeApi.cs` - a `[StructLayout(Sequential)]` struct of ~53
live `delegate* unmanaged<...>` fields (plus `FunctionInvoke` and
`ValueGetNetworkId`, which are currently dead and can be dropped). These become
the host-supplied imports. Groups:
- logging (`Log`, `FreeString`),
- value construction/accessors (`ValueCreate*`, `ValueGet*`, `ValueInvoke`),
- engine reflection (`Class*`, `Function*`, `Property*`, `GlobalFunction*`),
- invocation hot path (`FunctionCall`, `PropertyGet`, `PropertySet`),
- lifecycle (`EnvironmentCreate/Destroy`, `PackageReload`, `LibraryLoad`,
  `PackageGetEntity`, `LuaGetGlobal`, `LuaSetGlobal`).

### Exports: managed functions the engine calls
`NanosSharp/Interop/ManagedCallbacks.cs` - 11 `delegate* unmanaged<...>` fields
(`EngineAttached`, `EngineDetaching`, `Tick`, `EntityCreated`, `EntityDestroyed`,
`InvokeFunction`, `ReleaseFunction`, `PackageLoaded`, `PackageUnloading`,
`ClientFilesStoring`, `ClientFilesStored`), plus the single bootstrap entry
`Entry.Initialize(NativeApi*, ManagedCallbacks*, byte* runtimeDirectory)`.

### The data shape to redesign: `NativeArgument`
`NanosSharp/Interop/Arguments/NativeArgument.cs` - a 32-byte explicit-layout
tagged union carrying the `ValueType` tag plus the payload. Two parts of it are
not WASM-portable as-is and are the core of the marshalling work:
- **strings** are passed as a raw `(pointer, length)` into caller memory,
- **entities** are passed as `(classId: ulong, instance: nint, networkId: uint)`
  where `instance` is a raw engine pointer.

In WASM there are no shared host pointers: strings become offsets into the
module's linear memory (or a copy call), and every `nint` handle and entity
`instance` pointer must become an opaque integer index into a native-side table.

## Chattiness (the performance work)

- `FunctionCall` (entity/global method invocation) is already **one crossing per
  call**: all arguments packed into one contiguous `NativeArgument[]`, result by
  value. This path is fine.
- The **handle-based value API** (`ValueCreateArray`/`Map`, `PropertyGet`/`Set`,
  `ValueInvoke`, Lua globals) is O(N): marshalling an N-element collection is
  ~2N+1 crossings. Under WASM, host-call cost is the dominant factor, so this is
  the first thing to batch.
- `NativeClassRegistry.Load` at engine-attach reflects the entire scripting API
  with thousands of one-shot crossings. Under WASM this should become a single
  batched "dump schema" import.

> CORRECTION: an earlier version of this section reported ~305 ns/call and
> ~188 ns/value and concluded the chatty axis was a serious problem that only a
> core module could fix. Those numbers came from measuring the crossing through a
> **debug-built** Rust host, which inflated the host-side closure 10-150x. With
> the host built in **release** (`experiments/wasm-probe/crossing-bench.sh`
> enforces this) the conclusion reverses: wasm interop is cheap. The corrected
> numbers follow.

Measured (release hosts, `experiments/wasm-probe/crossing-bench.sh`), per guest
-> host crossing and per batched value:

| mechanism | ns/call | ns/value (batch) |
|-----------|---------|------------------|
| wasm core (NativeAOT) | ~6.0 | ~0.60 |
| wasm core (Mono) | ~6.4 | ~0.60 |
| Lua 5.4 -> C (baseline) | ~14 | - |
| wasm component (Mono AOT) | ~23 | ~1.27 |
| process sandbox spin / blocking | ~37 / ~3400 | - |

So wasm interop is competitive with or cheaper than Lua: a core crossing (~6 ns)
beats a Lua C call (~14 ns), a component crossing (~23 ns) is ~1.7x Lua.
Batching works on both (~0.6-1.3 ns/value). The component adds only ~17 ns/call
and ~0.6 ns/value over a raw core module - real but tiny; even 50k component
crossings/frame cost ~1.1 ms at 60 fps. Compute is unchanged: wasm AOT ~7.4x
faster than Lua 5.4.

Consequences that changed from the earlier wrong analysis:
- The chatty axis is **not** a blocker; the component penalty is small.
- NativeAOT vs Mono makes **no per-call difference** (~6 ns both via core host);
  NativeAOT's advantage is module size (2.6 MB vs 32 MB) and no bundled Mono, not
  interop speed. So the earlier "core module needed to fix interop performance"
  motivation largely evaporates - the Mono component path is already fast enough.
- Still prefer a coarse API over a chatty one, but as good practice, not a
  rescue: there is no perf cliff to design around.

## Open consequences of the component-model finding

The native client must embed a **component-model-aware** WASM runtime (wasmtime
C API or Rust crate - both support the component model and the WIT bindings the
game client would generate). The `NativeApi`/`ManagedCallbacks` tables become a
WIT world: imports = the engine functions, exports = the managed callbacks. The
`Wasmtime` .NET NuGet used in the first probe host only loads core modules and
is a dead end for this.

## The export direction (ManagedCallbacks): no reactor, so invert to guest-pull

The engine calls managed code (Tick, EntityCreated, InvokeFunction,
PackageLoaded, ...). That is naturally a set of guest *exports* the host calls
repeatedly after a one-time init - a WASI *reactor*. Finding: stock .NET 10 WASI
has no reactor output. `WasiApp.targets` always links `crt1-command.o` (the
command model: a single `_start` / `wasi:cli/run`), and `OutputType=Library`
produces only a managed `.dll`, no wasm at all. So custom host-callable exports
after init are not available out of the box.

Two workarounds, both viable:
- **Guest-pull inversion (chosen for the probe).** Keep the command model. The
  guest's entry runs an event pump that calls a host import `next-event` until
  it returns -1; the host (engine) queues events for it. This delivers host ->
  guest events into the running runtime with no reactor. Proven in the probe
  (`next-event`; the guest logs `handled event 10/20/30`).
- **Reactor-adapter relink.** Post-process the command core module with the
  wasi-sdk reactor adapter so it exports `_initialize` plus custom functions.
  Cleaner long-term (the engine calls `tick` directly each frame) but more
  toolchain work; deferred.

## Alternative isolation: separate-process sandbox (measured)

An OS-level alternative to WASM: run the client scripts in a separate
low-privilege process (Windows restricted token / Job Object / AppContainer;
~95% of players are on native Windows) and talk over shared-memory IPC. The
confinement is Windows-specific, but the performance-decisive part - the
synchronous crossing cost - was measured with equivalent primitives in
`experiments/ipc-probe`:

- busy-poll crossing ~37 ns, but it pegs a CPU core (a client cannot usually
  spare one, and the number assumes a free core),
- blocking (semaphore/event) crossing ~3400 ns - CPU-friendly but ~11x WASM and
  catastrophic for fine-grained synchronous calls (~3.4 us each).

So the process sandbox does not beat WASM for fine-grained synchronous script ->
engine calls unless a core is burned: the crossing is the bottleneck, as with
WASM. This weakens the case for a second, Windows-only sandbox built for
performance. Proton (~5%) is also not cleanly covered by OS sandboxing anyway:
the client is a Windows binary under Wine, which does not enforce Windows
security tokens as a real boundary, and a Linux-native sandbox around the Wine
process is not controllable from the runtime. WASM remains the one isolation
that works on both and burns no core.

## Open design decision: component model vs core module

The probe surfaced the real fork for the boundary, driven by host memory access:

- **Component model (typed WIT ABI).** The boundary is expressed in WIT types
  (`string`, `list<u8>`, records). The host never touches guest memory; the
  canonical ABI copies values across via the guest's `realloc`. Clean,
  typed, multi-language, but every `NativeArgument` shape must be re-expressed
  in WIT, raw pointers are not allowed, and string/collection crossings pay a
  copy. Scalars (the two `value-*` probe functions) work perfectly; `log` with a
  raw `s32` offset does not, because the host cannot read the offset without
  memory access - it would need `log: func(message: string)`.
- **Core module (raw-pointer ABI).** The host loads the pre-component core module
  and gets direct guest-memory access (wasmtime `Caller.get_memory`). This
  matches the current `NativeArgument` ABI almost unchanged (offsets into linear
  memory), so the port is mechanical, and the native client embeds core wasmtime
  (C API) plus WASI preview1 and supplies the import table directly. Gives up the
  typed component niceties.

Decision: core-module / raw-bytes ABI in substance. It matches the existing raw
ABI and keeps the marshalling port mechanical.

Toolchain reality found in the probe: .NET 10 WASI only targets wasip2 and emits
a component (no preview1 core module ships), so a pure preview1 core-module host
is not available. The raw-bytes intent is kept anyway by typing payloads as
`list<u8>` in WIT: the canonical lowering is still `(ptr: i32, len: i32)` - the
guest's raw `(byte*, int)` import is unchanged - and a component host receives
the bytes directly (the runtime reads guest memory for it). So the host still
gets raw bytes; only the WIT declaration differs. Scalars (handles) pass as
plain `s32`/`s64`. The real `NativeArgument` will likewise be carried as
`list<u8>`/scalars, not re-expressed as typed WIT records.

## Phases

1. Probe (done): prove runtime-in-wasm preserves reflection/DI. See
   `experiments/wasm-probe`.
2. Import-table build (DONE): a slice of `NativeApi` (`log`, one `ValueCreate`,
   one `ValueGet`) is modelled as a WIT world in
   `experiments/wasm-probe/wit/nanos.wit` and successfully injected into the
   .NET WASI component. Confirmed with `wasm-tools component wit`: the component
   now carries `import nanos: interface { log; value-create-integer;
   value-get-integer }`. How it was made to work:
   - the custom imports force a native relink, so `WASI_SDK_PATH` (wasi-sdk 25,
     clang 19) must be set,
   - a second `--component-type` pointing at `nanos.wit` is appended through a
     `<LinkerArg>` item; `wasm-component-ld` merges it with the built-in WASI
     world,
   - the WIT interface must be named `nanos` to match `[DllImport("nanos")]`,
     and the guest's `EntryPoint`s must be kebab-case (`value-create-integer`)
     to match WIT function names.
3. Import-table run (DONE): a Rust wasmtime component host
   (`experiments/wasm-probe/host-rs`) supplies the `nanos` interface plus WASI
   and drives the guest's `wasi:cli/run` export. End-to-end verified: the guest
   calls `value-create-integer`/`value-get-integer` (guest -> host -> guest round
   trip) and `log` (host receives the raw bytes), while reflection and DI run
   inside the sandbox. `experiments/wasm-probe/run.sh` reproduces it.
3. Handle/entity ABI (started): the `FunctionCall` argument-packing path is
   proven in the probe - the guest packs a count-prefixed `NativeArgument[]`
   (32-byte records + a scratch blob for strings, mirroring `ArgumentPacker`),
   crosses it as `list<u8>`, and the Rust host unpacks it faithfully including
   strings via scratch offsets (`function-call` in `host-rs`). Remaining:
   entities (classId/instance/networkId -> handle table indices) and the result
   direction (the single `NativeArgument` returned by value). The
   `ManagedCallbacks` host -> guest direction is handled by inversion (below).
4. Batch the chatty paths (value collections, schema dump).
5. AOT mode (DONE as far as feasibility: builds, runs, lifts the trampoline
   limit, reflection/DI survive trimming via rooting). Compute benchmark DONE
   (`experiments/wasm-probe/bench`): on an identical 64-bit loop (verified same
   result), C# wasm AOT is ~7.4x faster than Lua 5.4 (1.83 vs 13.55 ns/iter),
   ~25% over native JIT (1.46), while the wasm interpreter (18.04) is slower
   than Lua 5.4 - so AOT is the shipping mode and it beats the current client
   runtime. Remaining: make the real runtime's reflection trim-safe (root/
   annotate). Chatty-axis measured (release hosts): component ~23 ns/call, core
   ~6 ns/call, vs Lua ~14 ns - interop is cheap, not a blocker (see the
   corrected chattiness section).
6. Threading model and collectible `AssemblyLoadContext` hot reload inside wasm.
7. NativeAOT-LLVM core-module path (DONE as a spike): built a NativeAOT-LLVM
   command guest (`experiments/wasm-probe/naot`), extracted its core module and
   ran it under the direct-memory host. Findings: crossing ~6 ns/call, same as
   the Mono core module (~6.4 ns) - so NativeAOT gives **no per-call interop
   advantage**; the earlier expectation of a big Mono-thunk saving was wrong (it
   was the debug-host artifact). NativeAOT's real wins are **module size (2.6 MB
   vs Mono's 32 MB) and no bundled Mono**. Its cost is the NativeAOT reflection /
   dynamic-loading limits (no `Reflection.Emit`, no `Assembly.LoadFile`), which
   our DI/eventing/hot-reload lean on - a managed-side rewrite. Since interop is
   already cheap on the Mono component path, NativeAOT is now a footprint/startup
   decision, not a performance necessity.

## Research findings (web, 2026) - a supported core-module path exists

Web research (sources below, dates noted; treat as leads to verify, not yet
reproduced here) clarified the root cause and a production-grade path:

- The component-only output is a deliberate Mono decision: dotnet/runtime
  PR #104683 (2024-07-12) switched the Mono `wasi-experimental` path to emit a
  WASI Preview 2 component instead of a core module, with no opt-out. That is
  exactly why we pay the canonical-ABI copy.
- **NativeAOT-LLVM** (`Microsoft.DotNet.ILCompiler.LLVM`, the runtimelab
  successor) emits a **core module by design**: reactor-style
  (`[UnmanagedCallersOnly]` exports, no `main`), custom host imports via
  `[WasmImportLinkage]`+`[DllImport]` with raw pointers / by-value structs
  straight into linear memory, and **no Mono runtime**. (We then measured it:
  per-crossing cost is ~6 ns, the same as the Mono core module - the interop win
  we expected from dropping Mono did not materialise, because interop was never
  actually slow; the earlier slowness was a debug-host artifact. NativeAOT's real
  payoff is a 2.6 MB module vs Mono's 32 MB.)
- Precedent: **SpacetimeDB** runs sandboxed C# game logic this way in production
  (NativeAOT-LLVM core module, custom `spacetime_*` host ABI, direct pointer
  marshalling; they abandoned the Mono-AOT hack as "too fragile"). This is the
  same shape as our `host-core-rs` probe, but with a supported compiler instead
  of `wasm-tools unbundle`.

The catch - this is the real trade now: **NativeAOT-LLVM inherits NativeAOT
limits.** No `Reflection.Emit`, no `Assembly.LoadFile`/dynamic loading, trimming
mandatory, Win-x64 + Linux-x64 only (no macOS - fine, the game is Windows +
Proton/Linux-x64). Our runtime leans on exactly the dynamic features NativeAOT
forbids: `CallbackInvoker` compiles expression trees (Reflection.Emit), the
package loader uses collectible `AssemblyLoadContext` (dynamic loading +
hot-reload), and DI/event scanning is reflection-driven. So the fast path costs
a managed-side rewrite to be AOT-safe (source generators instead of
Reflection.Emit; a different reload story than collectible ALC; trim-rooting).

So the two client paths are now defined - and with the corrected numbers the
interop gap between them is small, so the choice is really about dynamic C#
features, module size, and maturity, not interop speed:

| path | interop perf | module size | dynamic C# (Reflection.Emit, dynamic load, hot-reload) | maturity |
|------|-------------|-------------|--------------------------------------------------------|----------|
| Mono component (validated here) | ~23 ns/call, ~1.3 ns/value (fine) | ~32 MB | full | wasi-experimental, works today |
| NativeAOT-LLVM core (SpacetimeDB precedent) | ~6 ns/call, ~0.6 ns/value | ~2.6 MB | cut (no Emit/dynamic load; reflection must be AOT-safe) | experimental feed, Win/Linux-x64 |

Sources: dotnet/runtime#104683 (2024-07-12); SpacetimeDB#4514 (2026-03-02) and
v2.10.0 notes; bytecodealliance/componentize-dotnet; "The Road to Component
Model 1.0" (2026-06-08, names the per-value copy cost and the ~3.5x async
bookkeeping, and that true host<->guest shared memory is not on the component
roadmap - it only exists on the core-module path); dotnet/runtime#96419 (WASI
roadmap, stalled at "Future" since 2024-12).

## Stage 1 (started): client runtime builds for wasi-wasm, seam over imports

Done and verified under a standalone wasmtime dev host
(`managed/NanosSharp.WasmRuntime/devhost`):

- The real `NanosSharp` (Client) assembly builds for `wasi-wasm` (Mono, AOT +
  trimming, rooted) via an additive `-p:NanosSharpClientRuntime=Wasm` flavor in
  `managed/Directory.Build.props`. SERVER and the normal Client build are
  untouched.
- The `NativeApi` seam is backed by wasm imports with zero changes to the 44
  call sites: `NanosSharp/Interop/Wasm/WasmBootstrap.cs` (compiled only under
  `NANOSSHARP_WASM`) fills a `NativeApi` whose `Log` field is `&LogThunk`, an
  `[UnmanagedCallersOnly]` thunk forwarding to a `[WasmImportLinkage]` import.
- `Entry`-equivalent boot runs: `NativeRuntime.Initialize` + repeated
  `NativeRuntime.Log` round-trips print through the host, clean return. The thin
  `NanosSharp.WasmRuntime` exe is just the wasi entry calling
  `WasmBootstrap.Run`.

Blocker found and deferred (Stage 2): the `ManagedCallbacks` dispatch
(`callbacks->Tick`, etc.) and value-returning host imports trap under the Mono
wasi runtime. Cause, narrowed precisely: this code runs in Mono's **interpreter**
(mixed mode), and the interpreter's interp-to-native trampoline set only covers
integer-argument / void-return signatures - float arguments (Tick takes a
`float`) and value returns hit `aot-runtime-wasm.c:188 <disabled>`. The same
signatures work when the caller runs AOT (proven in `experiments/wasm-probe`).
Integer/void imports (Log) work fine, which is why the seam proof passes. Two
clean resolutions for Stage 2: force the boot/dispatch tree to execute AOT (needs
Mono-wasi investigation - putting it in the AOT'd runtime assembly was not
sufficient), or move the client runtime to the NativeAOT-LLVM core-module path,
which has no interpreter and so cannot hit this trampoline limit at all.

## Stage 2 (started): NativeAOT-LLVM path resolves the blocker

The user chose the NativeAOT-LLVM path. A second additive build flavor
`-p:NanosSharpClientRuntime=WasmAot` was added (`managed/Directory.Build.props`
+ `Microsoft.DotNet.ILCompiler.LLVM` 10.0.0-rc.1 package refs in
`NanosSharp.WasmRuntime.csproj` gated on `WasmAot`, plus `managed/nuget.config`
adding the dotnet-experimental feed). It compiles the real `NanosSharp` (Client)
runtime with NativeAOT-LLVM (no Mono, no interpreter), producing a ~9.9 MB
component (vs Mono's ~30 MB) at `.../wasi-wasm/native/NanosSharp.WasmRuntime.wasm`
(NativeAOT output is under `native/`, not the Mono `AppBundle/`, because
`WasmGenerateAppBundle=false`).

Verified under the dev host: the exact `ManagedCallbacks` dispatch that trapped
under Mono now runs cleanly - `Initialize`, then three `callbacks->Tick(0.016f)`
each reaching the managed Tick handler via `OnTick -> RunTick -> Tick event`,
then clean return. NativeAOT-LLVM removes the interp-trampoline limitation
entirely, as predicted. The build emits ~24 IL2026/IL2070/IL3050 trim/AOT
warnings on the reflection paths (`CallbackInvoker`'s `Expression.Compile`, DI,
attribute scanning); they are dormant until those paths run, and making them
AOT-safe (source generators instead of `Reflection.Emit`, no collectible ALC) is
the remaining Stage 2 work, along with the real `NativeArgument` marshalling and
loading prebuilt package assemblies.

## Milestone: a runtime-delivered package runs sandboxed and calls the engine

Reached on the Mono-wasm path (the probe package project lives under
`managed/NanosSharp.WasmRuntime/testpackage`, a normal net10 class library
compiled separately, not baked into the module). The Mono-wasm runtime boots,
then `WasmBootstrap.LoadPackages` reads a package `.dll` from a host-mounted WASI
directory (`/package`), `Assembly.Load`s it, finds the entry type by reflection,
instantiates it and runs `Start()`. The package's C# calls `Log.Success(...)`,
which flows through `NativeRuntime.Log` and the `nanos` import to the host:

```
loading package WasmSmokePackage.dll
starting WasmSmokePackage.Entry
[guest/11] hello from a runtime-loaded package
[guest/11] this C# executed inside the wasm sandbox
```

This is the defining capability: **server-delivered C# runs sandboxed on the
client and reaches the engine API**, end to end.

Two findings this locks in:
- **The client runtime must be Mono, not NativeAOT.** NativeAOT cannot load IL
  assemblies at runtime (no JIT, no `Assembly.Load` of new code), so it cannot
  run arbitrary server-delivered packages - it only fits a build-time-fixed set.
  Mono-wasm loads and interprets package IL at runtime (the runtime itself is
  AOT'd), which is exactly what client scripting needs. NativeAOT remains the
  value for proving the callback dispatch and for any fixed-at-build scenario.
- **The value-returning-import-from-interp blocker still gates full function.**
  Package code runs interpreted (loaded at runtime), and integer/void imports
  work (`Log.Success` -> `NativeRuntime.Log`), but the moment a path calls a
  value-returning import it traps: `Log.Info` goes through
  `NativeRuntime.IsEngineAttached` -> `EngineIsAttached()` (`() -> int`) and hits
  the Mono interp-to-native trampoline limit. So the remaining Stage 2 work is
  unavoidably to solve this: it blocks most of `NativeApi` (every `ValueGet*`,
  `FunctionCall`, property/method access returns a value). Candidate directions:
  a precompiled trampoline set for the needed signatures, an all-void/handle ABI
  reshaping, or driving the hot paths through AOT'd runtime code rather than
  interpreted package code.

## Blocker SOLVED: direct named imports get working trampolines

Diagnosis refined and fix proven: the trap is not value-returns per se, it is
*how* the call is made. Mono-wasm emits a working per-signature interp-to-native
trampoline for every **direct named `[WasmImportLinkage][LibraryImport]`
pinvoke** it sees, but calls through the `delegate* unmanaged` function-pointer
struct (`_api->Field(...)`) only use a limited default trampoline set that covers
integer/void but not value-returns. That is why `Log` (int/void, via the struct)
worked while `EngineIsAttached` (`() -> int`, via the struct) trapped.

Fix: in the wasm build, have `NativeRuntime` call a direct named import instead of
the struct field. Done for `IsEngineAttached` (`NativeRuntime.cs`, `#if
NANOSSHARP_WASM` -> `[LibraryImport("nanos","engine-is-attached")]`). Verified:
the runtime-loaded package now calls `Log.Info`, which goes through
`NativeRuntime.IsEngineAttached` -> the value-returning import, and it returns
cleanly from interpreted package code - no trap.

So the whole value-returning `NativeApi` is reachable from interpreted packages
by routing each of the ~53 functions through a direct named `nanos` import in the
wasm `NativeRuntime` (mechanically generable from `NativeApi.cs`), rather than the
function-pointer struct. This unblocks the rest of Stage 2 (real `NativeArgument`
marshalling, `ValueGet*`, `FunctionCall`, properties) on the Mono client.

## Milestone: full generated seam + runtime package, end to end

The whole `NativeApi` seam is now generated for the wasm build by
`managed/NanosSharp.WasmRuntime/tools/gen_wasm_nativeapi.py`, which parses
`NativeApi.cs` and emits `NativeApi.Wasm.g.cs` (a `#if NANOSSHARP_WASM partial
struct NativeApi` whose 58 methods forward to direct
`[WasmImportLinkage][LibraryImport("nanos", ...)]` statics) plus the matching
`wit/nanos.wit`. The fnptr fields in `NativeApi.cs` are gated to `#if
!NANOSSHARP_WASM`, so the 44 call sites (`NativeRuntime.Api.X(...)`) are
unchanged - on the wasm build `X` is now a method backed by a named import, which
gets a working interp-to-native trampoline.

Verified end to end on the correct architecture (core module + direct memory):
the component is unbundled to its core module and run under `devhost-core`
(preview1 WASI + a direct-memory host that reads/writes guest linear memory). A
runtime-delivered package loads and runs, and its `Log.Info` now works - that
single call exercises both a **value-returning import** (`engine-is-attached`,
which trapped before) and a **raw-pointer import** (`log(level, ptr, len)`, where
the host reads the string straight out of guest memory). Output:

```
loading package WasmSmokePackage.dll
starting WasmSmokePackage.Entry
[guest/5] hello from a runtime-loaded package (via Log.Info)
devhost-core: guest returned cleanly
```

So the complete Mono client interop model is proven functional: runtime package
loading (Mono) + the full named-import seam (value-returns work) + core module
with host memory access (pointer marshalling works). What remains is the real
`NativeArgument` string/entity marshalling conventions on top of this seam, the
batched schema load, and Stage 3 (the native wasmtime embed in `native/`, which
plays the role `devhost-core` plays here). The raw seam needs the direct-memory
core-module host; the component dev host only fits integer/void + list<u8>.

## Milestone: real argument marshalling, both directions

The real `ArgumentPacker` and `ResultReader` now round-trip through the seam under
`devhost-core`, not just scalars. `WasmBootstrap.StringRoundTripSelfTest` packs a
string argument with the production `ArgumentPacker.Add(string)` (UTF8 into the
guest scratch, `StringData` set to the guest-memory offset, `StringLength` next to
it in the 32-byte `NativeArgument`), hands the argument buffer to `function-call`,
and the host reads the string straight out of guest linear memory at that offset.
For the return path the host writes UTF8 into a guest result scratch and fills a
`ValueType.String` `NativeArgument`, which the production `ResultReader.Read` then
decodes. Output:

```
integer result round trip: ok=1 result=42
[host] string arg: type=3 'hello from guest'
string round trip: ok=1 result='pong from host'
```

This confirms the 32-byte explicit layout survives wasm32 (where `nint`/pointer
fields are i32 offsets into guest memory, not host pointers) for both an argument
the guest writes and a result the host writes. The self-tests are guarded by
`!NativeRuntime.IsEngineAttached`, so they only run under the dev host and never
fire a bogus `function-call` at a real engine.

## Milestone: batched schema load + handle-table-index convention

`NativeClassRegistry.Load` on the wasm build no longer walks the engine with
thousands of per-class/per-method/per-property crossings (each of which also
returned a native name pointer). It now makes **one** `schema-dump` crossing: the
host serializes the whole class/function/property schema into a single blob in
guest memory, and the production registry parser rebuilds the real `NativeClass`,
`NativeMethod` and `NativeProperty` objects from it (via new pre-read constructors
that take the already-decoded name instead of calling `*GetName`). Verified with a
synthetic two-class schema:

```
class Player id=1001 methods=2 static=1 properties=1 ctor=Constructor
class Vehicle id=1002 methods=1 static=0 properties=0 ctor=none
global function GetWorldTime
```

This also pins down a convention that the generated seam already forces: every
engine handle that crosses to the guest is an **i32 table index**, not a raw
pointer. The seam lowers every `nint` to `s32`, so a 64-bit host pointer could not
survive the crossing; the host (devhost-core now, the native embed later) owns the
tables and the guest only ever holds and passes back indices. The blob carries
those i32 handles for constructors, methods and properties, so later
`function-call` / `property-get` calls route back through the host table. The
per-handle path is untouched behind `#else` for the in-process server build.

Format is length-prefixed and little-endian (`u32` class count; per class: `u64`
id, string name, `i32` constructor handle plus its name when non-zero, then `u16`
counts and `(i32 handle, string name)` records for instance methods, static
methods and properties; then a `u32` global-function count with the same records).
Strings are a `u16` byte length followed by UTF8. `gen_wasm_nativeapi.py` keeps
`schema-dump` in the generated WIT as a wasm-only import so regeneration preserves
it. Signatures stay lazy (fetched by handle only when actually read), so the blob
stays small.

## Stage 3, slice 1: the native wasmtime embed

Everything above runs the sandbox under the standalone `devhost-core` (Rust).
Stage 3 moves that host into the real native tree as production C++ so it can run
embedded in the game. The first slice (`native/src/wasm/`, committed) proves the
embed path end to end against the real engine seam, validated without the game by
a mock-engine harness:

- `WasmHost` (`wasm_host.{h,cc}`) uses the **wasmtime C API** (not the dead-end
  Wasmtime .NET package) to compile and instantiate the unbundled Mono core
  module, wiring WASI preview1 with `/package` preopened read-only, and binding
  the `nanos` imports to a `const bridge::NativeApi&` - the exact seam
  `GetNativeApi()` already returns in-process. It owns the handle tables (i32
  index to native `IFunction*`/`IProperty*`), reads and writes guest linear memory
  through `wasmtime_memory_data`, and defines any remaining module imports as traps
  (the C API has no `define_unknown_imports_as_traps`, so we enumerate
  `wasmtime_module_imports` and trap what the linker has not already bound, plus a
  `adapter_close_badfd` stub).
- `log`, `engine-is-attached`, `free-string`, `schema-dump` and `function-call`
  are implemented for real. `schema-dump` walks `class_count`/`class_get`/... once
  and serializes the same blob the guest parser expects, interning each handle to
  an i32. `function-call` field-translates each guest 32-byte `NativeArgument`
  (i32 offsets, wasm32 layout) into the native 64-bit `bridge::NativeArgument`
  (resolving a string argument's `StringData` offset to a `const char*` into guest
  memory), calls the seam, and writes the scalar result back into guest memory.
- `wasm_host_test.cc` is a `nanossharp-wasmhost-test` binary (xmake, kind binary,
  not built by default) that supplies a mock `NativeApi` with opaque class/function
  handles (Player/Vehicle plus a `GetWorldTime` global) and drives `WasmHost`
  against the real core module. It reproduces the exact `devhost-core` output
  through the production C++ path:

```
integer result round trip: ok=1 result=42
[host] string arg length=16 'hello from guest'
string arg round trip: ok=1 length=16
class Player id=1001 methods=2 static=1 properties=1 ctor=Constructor
class Vehicle id=1002 methods=1 static=0 properties=0 ctor=none
global function GetWorldTime
hello from a runtime-loaded package (via Log.Info)
```

Build wiring: an xmake `wasmtime_dir` option points at the extracted wasmtime C
API (default `~/.cache/nanossharp-wasmtime/wasmtime-v29.0.1-x86_64-linux-c-api`);
the test target static-links `libwasmtime.a` via `ldflags` (after the objects) so
the binary carries no `libwasmtime.so` dependency. The test target compiles with
`_GLIBCXX_USE_CXX11_ABI=0` like `nanossharp`, because `game/script_value.h` asserts
that ABI. SERVER and the existing `nanossharp` shared library still build cleanly.

The string self-test was simplified to return the argument's byte length as an
integer (both hosts), dropping the earlier result-string scratch hack; the
production convention for host-written result strings moves to slice 2.

What this slice deliberately leaves for later slices:
- the rest of the seam: the `value_*` and `lua_*` surface, entity handle tables
  (`class_id`/`instance`/`network_id` across the boundary), and the result-string
  scratch convention;
- the callback direction (engine to guest): in-process `ManagedCallbacks` are
  function pointers, but in wasm the host must call guest exports, so `tick`,
  `engine_attached`, `entity_created` etc. need an export-and-dispatch path;
- the actual game wiring: branching `Runtime::StartManaged` (the hostfxr path in
  `native/src/platform/{linux,windows}/runtime.cc`) to the wasm embed, delivering
  the unbundled module, and the Windows (Proton) build of the wasmtime C API.

## Stage 3, slice 2: the callback direction, and a hard Mono-wasi limitation

The engine to guest direction (tick, engine-attached, entity events) is where the
in-process model breaks for wasm. In-process these are `ManagedCallbacks` function
pointers the engine calls; in wasm the engine (native host) must instead call
**named guest exports**. The guest side of that is built and works: `WasmExports`
(`Interop/Callbacks/WasmExports.cs`) declares
`[UnmanagedCallersOnly(EntryPoint = "nanos_tick" | "nanos_engine_attached" |
"nanos_engine_detaching")]` entry points that forward to the existing dispatch
(`NativeCallbacks.DispatchTick` etc., extracted so both the in-process fn-pointer
path and the exports share one body). Confirmed with `wasm-tools`: the unbundled
core module really exports `nanos_tick`, `nanos_engine_attached`,
`nanos_engine_detaching`. `WasmHost` gained `Tick` / `EngineAttached` /
`EngineDetaching` that resolve and call these exports via `wasmtime_func_call`.

The hard part was the runtime lifecycle. The native host drives the engine, so it
must call these exports **repeatedly, synchronously, on the main thread**, after
boot - a game tick calls into the live runtime and the script's engine calls run
synchronously before the tick returns. That requires the runtime to survive across
host calls: a **WASI reactor** module (`_initialize` once, exports callable
after). Mono-wasi through the SDK only builds **command** modules: `_start` runs
the managed `Main` once via `mono_runtime_run_main` and returns, after which the
runtime is no longer callable. Calling any guest export post-`_start` traps with
`wasm trap: uninitialized element` (the native-to-managed dispatch trampoline is
never set up).

Dead ends for getting a reactor out of the SDK directly:
- `OutputType=Library` plus a manual `UsingMicrosoftNETSdkWebAssembly=true`, and
  even forcing `_IsLibraryMode=true`: the WASI link still used `crt1-command.o` +
  `main.o`, no `-DWASM_LIBRARY_MODE=1`.
- Building with `Sdk="Microsoft.NET.Sdk.WebAssembly"` (the Blazor SDK that sets
  `UsingMicrosoftNETSdkWebAssembly`) with `RuntimeIdentifier=wasi-wasm`,
  `OutputType=Library`: still linked `crt1-command.o`. Library/reactor mode is a
  browser-wasm feature; the SDK's wasi-wasm link path is hard-wired to command.
- A managed `&WasmExports.Tick` address-of during `_start` to warm the trampoline:
  the managed `delegate*` path is not the wasm export entry path, so it traps the
  same way.

The threaded alternative (guest `_start` never returns and runs an event loop fed
by the host on another thread) is **rejected**: nanos-world drives hooks
synchronously on the main thread and expects script side effects within the tick,
so a worker-thread guest would have to marshal engine calls back to a main thread
that is blocked waiting for the tick (deadlock), or run them asynchronously
(races). It does not fit a single-threaded engine.

### Solution: a custom reactor link

The reactor the SDK will not emit is built by hand, reusing the object files the
SDK already produced. `tools/relink_reactor.py` takes the SDK's command link,
recompiles the Mono runtime `main.c` with `-DWASM_LIBRARY_MODE=1` (so it provides
`initialize_runtime` and no `main()`), and relinks with `wasm-ld` using
`crt1-reactor.o` and `--no-entry`, exporting `_initialize`, `initialize_runtime`
and the managed `[UnmanagedCallersOnly]` entry points. `tools/build-reactor.sh`
wraps the whole flow (publish, capture the expanded link command, relink) into one
command. The boot logic moved from `Main`/`_start` into a `nanos_boot` export
(`WasmBootstrap.Boot`), since a reactor has no `_start`. `WasmHost::RunStart` now
drives the reactor init sequence: `_initialize`, then `initialize_runtime`, then
`nanos_boot`.

Proven end to end against the real `NanosSharp` runtime: the module boots through
that sequence, runs the imports-direction self-tests and loads the package, then
the host calls `nanos_tick` three times and `nanos_engine_detaching`, each
dispatching into the **still-live** runtime (`DispatchTick` -> `RunTick` -> the
`Tick` event):

```
wasm runtime ready
tick delta=0.016
tick delta=0.016
tick delta=0.016
wasm-host-test: host-driven callbacks completed
```

A minimal reactor probe also confirmed state persists across host calls (a static
counter accumulated 10 then 15 over two export calls), i.e. the runtime genuinely
stays alive, not re-initialized per call.

So both interop directions now work through production code against a reactor
module, and the Mono client (with runtime IL package loading, the whole
plug-and-play point) stays the basis.

Caveats and follow-ups. The relink is an unofficial build path that bypasses the
SDK's command link, so it needs maintaining across .NET updates. It currently runs
the reactor **interpreted** (roughly Lua-level per the earlier benchmarks), not
AOT. `relink_reactor.py` can rebuild an AOT module - it regenerates the
per-assembly objects the SDK writes to `/tmp` and deletes, replaying their compile
commands from the publish log - and that produces a valid ~31 MB AOT reactor. But
it traps at the first managed export (`nanos_boot`, `uninitialized element` right
at entry). The exact mechanism, from the generated `pinvoke-table.h`: the
`nanos_boot` export wrapper is
`if (!wasm_native_to_interp_ftndescs[i].func) mono_wasm_marshal_get_managed_wrapper(...);
((InterpEntry)wasm_native_to_interp_ftndescs[i].func)(...)`. The wrapper only knows
the **interpreter-entry** path; it expects the first call to fill the ftndesc via
`mono_wasm_marshal_get_managed_wrapper` -> `mono_compile_method`. In the AOT reactor
that fill does not happen, so the final indirect call targets a null slot. It works
in the interpreter build because the method is genuinely interpreted and the fill
succeeds; under AOT the ftndesc stays null.

A warmup was attempted to fix it: inject a call that runs the (empty) entry-assembly
`Main` once, the way the command path's `mono_runtime_run_main` warms the runtime -
first from the SDK's `WASI_AFTER_RUNTIME_LOADED` in-init hook, then from a separate
host-called export after `_initialize`+`initialize_runtime` fully return. Both hit
the same Mono assertion `object.c:4301` inside `mono_runtime_run_main`, i.e.
`run_main` is not valid in the reactor execution context regardless of timing. So
the command-style warmup is not a viable fix from the embedder side; the experiment
was reverted to keep the working interpreter reactor. Unblocking AOT+reactor looks
like it needs Mono-wasi-internal work (a supported reactor warmup, or excluding the
`[UnmanagedCallersOnly]` export methods from AOT so they stay interpreted and the
ftndesc fill path runs), which is an upstream/runtime task rather than an embedder
one. The interpreter reactor remains the shippable baseline.

The other follow-ups are wiring the relink into the real game delivery, the rest of
the `value_*`/`lua_*`/entity seam (now done, see slices 4-5), and the Windows
(Proton) wasmtime C API build.

## Stage 3, slice 3: result-string marshalling

The one marshalling gap left open earlier - a host writing a **string result** back
to the guest - is now closed with a registered scratch convention. A guest
`NativeArgument` string result cannot point at host memory, so at boot the guest
allocates a persistent native buffer and hands its offset to the host via a
wasm-only `set-result-scratch(buffer, capacity)` import (`WasmResultScratch`,
generated into the WIT alongside `schema-dump`). When `function-call` returns a
string, `WasmHost` copies the bytes into that scratch and sets the result's
`StringData` offset and `StringLength`; the production `ResultReader.ReadString`
decodes it. It is consumed synchronously before the next call reuses the scratch,
which is safe because results are read into managed strings immediately. Verified:

```
string result round trip: ok=1 result='pong from host'
```

With this, `function-call` marshals every non-handle type both ways: bool,
integer, double and string explicitly, and vector/rotator/color/vector2d by raw
copy (their layout is identical across the boundary, no pointer fields).

## Stage 3, slices 4-5: handle tables for values and entities

The pointer-bearing handle types are now marshalled too, each through a host-side
handle table mapping an i32 the guest holds to the real native pointer - the same
trick the seam already forces for class/function/property handles (every `nint`
lowers to `s32`, so a 64-bit host pointer cannot cross; the host owns the tables).

`ValueHandle` (a `ScriptValue*`): `WasmHost` keeps a value table and implements the
scalar `value_*` surface - `value_create_nil`/`bool`/`integer`/`double`, `clone`,
`free`, `get_type`/`bool`/`integer`/`double`. A create interns the returned pointer
to an i32; a get/free resolves the i32 back. This needed a typed functype builder,
since these imports are not all-i32 (`value_create_integer` is `(i64)->i32`,
`value_get_double` is `(i32)->f64`). Verified:
`value handle round trip: integer(type=1, value=123) bool=1 double=3.5`.

Entities: an entity `NativeArgument` carries `class_id`, `instance` and
`network_id`, where `instance` is a host pointer. `WasmHost` keeps an instance
table; on an entity argument it maps the guest i32 back to the real pointer, and
on an entity result it interns the pointer to an i32 before writing the guest
record. The guest side runs the real `ResultReader.Read` -> `EntityRegistry.Resolve`,
which finds the class in the (schema-loaded) registry and builds the wrapper.
Verified against the mock returning class 1001: `entity result round trip: ok=1
class=Player network=7`.

So `function-call` now marshals every type both ways: bool/integer/double/string
explicitly, vector/rotator/color/vector2d by raw copy, and the handle types
(`ValueHandle`, entity) through their tables.

The property surface is wired on top of these tables: `property_get`/`property_set`
plus `value_create_entity` let the real `NativeProperty.Get`/`Set` run through the
whole production stack - `NativeValueList` builds `ValueHandle`s with
`ValueMarshaller.ToNative` (the entity self goes through `value_create_entity` and
the instance table, a passed value through `value_create_integer`), the host
resolves the property handle from the schema-dump table, runs it, and interns the
result `ValueHandle` for `ValueMarshaller` to unwrap. Verified against the mock's
`Health` property: `property get round trip: Health=150` and a set of `200` arriving
at the host as `type=1 integer=200`.

String and geometry values are wired too, exercised through a real
`ValueMarshaller.ToNative`/`TakeFromNative` round trip: `value_create_string` reads
the guest string, `value_get_string` writes the bytes into the result scratch and
the length into the guest out-parameter, and `value_create_vector` /
`value_get_doubles` cover a value written into a guest output buffer. Verified:
`value marshaller round trip: string='hello values' vector=Vector(1.5, 2.5, 3.5)`.

With this, every distinct marshalling **pattern** is covered: scalars, strings both
ways, the value and instance handle tables, entities, properties, and the
out-parameter write. What is left is pure breadth over those patterns: the
remaining geometry creators/getters (rotator, color, vector2d - identical to
vector), the composite `value_*` (array, map, function, `value_invoke`, which add
reading a guest handle array and the function-callback path) and the `lua_*`
globals. None of these need a new mechanism.

## Benchmarks of the finished reactor

Measured on the finished interpreter reactor through the production `WasmHost`
(release wasmtime C API embed, `nanossharp-wasmhost-test ... bench`, stable across
runs). Each figure is one per-operation cost; the inner loop runs inside the
sandbox so a single timed export call divided by the iteration count isolates the
cost.

| Operation | Cost |
|---|---|
| compute loop (interpreter) | ~13 ns/op |
| import crossing, guest to host (`engine-is-attached`, no memory) | ~80 ns/op |
| value round trip (`create`+`get`+`free`, 3 crossings) | ~108 ns/op (~36 ns/crossing) |
| `function-call` (0 args, int result, full `NativeArgument` marshalling) | ~210 ns/op |
| host to guest export call | ~145 ns/op |

Context, from the earlier release probe (`experiments/wasm-probe`): Lua 5.4 compute
~13.55 ns/iter, wasm interpreter compute ~18 ns, wasm AOT compute ~1.83 ns (7.4x
faster than Lua); Lua to C crossing ~14 ns, raw wasm core crossing ~6 ns.

Honest reading of these numbers:
- **Compute** on the interpreter is Lua-level (~13 ns here vs Lua ~13.55 ns). AOT
  would be roughly 7x faster.
- **Crossings** on the interpreter are **slower than Lua** (~80 ns per guest-to-host
  import, ~36 ns per value crossing, vs Lua to C ~14 ns). The cost is the Mono
  interpreter-to-native trampoline, not the raw wasm crossing (~6 ns). With AOT the
  calls compile and crossings drop toward that raw number, i.e. below Lua.

So the finished **interpreter** reactor is a correct, working baseline that is
roughly Lua-comparable overall (Lua-level compute, more expensive crossings) while
adding the real isolation boundary and full C# (reflection, DI, runtime IL package
loading). The performance *win* that motivated C# over Lua - both faster compute
and cheaper crossings - is realized only with **AOT**, which remains gated on the
AOT+reactor init problem documented in slice 2. The practical conclusion: the port
is functionally complete and shippable as an isolation win at Lua-comparable speed;
unlocking the speed win is the AOT+reactor follow-up.

## Alternative host: a managed Wasmtime.NET embed (validated)

An alternative to embedding wasmtime natively in `native/` (the C++ `WasmHost`):
embed it in the **managed** layer via the `Wasmtime` .NET package. The native
layer stays exactly as it is today (version.dll -> hostfxr -> CoreCLR runs the
trusted `NanosSharp.Host`); that trusted host then uses Wasmtime.NET to run the
**untrusted package code** in a wasm sandbox, brokering engine calls through the
`nanos` seam it already holds. Only package code is sandboxed; the trusted broker
stays full-speed CoreCLR. (DotNetIsolator demonstrates the shape, but is
experimental, unsupported, and marshals every crossing with MessagePack copies -
too slow for crossing-heavy scripting - so this is the idea, not that library.)

This was validated with a probe (`experiments`, Wasmtime 22.0.0): the probe loads
the exact same reactor core module, defines the `nanos` imports as C# callbacks
via `Linker.DefineFunction` (enumerating `Module.Imports`, preview1 via
`DefineWasi`, the preview2 interfaces the component pulled in stubbed like the C++
host traps them), boots it (`_initialize` / `initialize_runtime` / `nanos_boot`)
and drives `nanos_tick`. It works, and the microbenchmarks against the native host
(interpreter reactor, same module) are:

| Operation | native C++ host | managed Wasmtime.NET host |
|---|---|---|
| compute | ~13 ns | ~12 ns |
| import crossing | ~80 ns | ~159 ns |
| value round trip (3 crossings) | ~108 ns | ~198 ns |
| function-call | ~210 ns | ~322 ns |

Compute is identical (it runs in the guest). The managed host roughly doubles the
crossing cost - the extra wasm -> Wasmtime.NET native -> managed-delegate hop - but
it **removes the hardest remaining blocker**: no native wasmtime embed, no Windows
(Proton) wasmtime C API, no version.dll wasm wiring. Wasmtime.NET ships native
wasmtime for win-x64/linux-x64/osx, so it is cross-platform for free, and the
integration is managed C# in `NanosSharp.Host` (the proven `WasmHost` design -
reactor module, handle tables, direct marshalling - ported from C++ to C#). It is
also the natural evolution of today's architecture, which already runs
`NanosSharp.Host` on CoreCLR on the client; we just add the package sandbox inside
it. AOT remains blocked either way (a guest property, host-independent).

Trade: ~2x crossing cost for dramatically simpler, cross-platform, managed wiring.
Since the interpreter baseline is not a Lua perf win regardless (the win needs AOT),
the simpler shippable path is the stronger option; the native C++ host stays
available if crossing cost ever dominates.

## Milestone: the managed Wasmtime.NET host is ported and validated

The C++ `WasmHost` is now ported to managed C# as `managed/NanosSharp.Sandbox`
(`WasmHost.cs`), the real embed the trusted host will use - not a probe. It mirrors
the C++ design one to one: an `Engine`/`Store`/`Linker`/`Module`/`Instance`, the
i32-to-native handle tables (functions, properties, values with a free list,
instances, environments), `DefineImports` enumerating `Module.Imports` (the `nanos`
interface routed to a single `Dispatch` switch, preview1 via `DefineWasi`, every
other import stubbed), and the full `Dispatch` surface: `log`, `schema-dump`
(the batched blob), `function-call` with per-field guest/native `NativeArgument`
translation, all `value-*`, `property-get`/`set`, `lua-*`, `environment-*`,
`library-load`, and the result-scratch string path. Guest linear memory is reached
with `Memory.GetPointer()`; the i32 handles the guest holds are indices into the
host-side tables, exactly as in C++.

A harness (`managed/NanosSharp.Sandbox.Harness`) ports the C++ mock engine
(Player/Vehicle classes, global `GetWorldTime`, `function-call`, property, all
`value-*`) to C# `[UnmanagedCallersOnly]` callbacks and drives the same reactor
module. It reproduces the C++ harness output exactly: every boot self-test
(integer, string arg/result, value handle, value marshaller for string/vector/
array/map, class schema, entity, property get/set) passes, the runtime-delivered
`WasmSmokePackage.dll` loads and its `Entry` runs, and the three host-driven ticks
plus detach complete. (The mock omits `environment-create`, so the harness drives
tick + detach without `nanos_engine_attached`, matching the C++ harness, which
skips it for the same reason.)

Benchmarks (interpreter reactor, release, same module as the native host):

| Operation | native C++ host | managed Wasmtime.NET host |
|---|---|---|
| compute | ~13 ns | ~13.7 ns |
| import crossing (engine-is-attached) | ~80 ns | ~176 ns |
| value round trip (3 crossings) | ~108 ns | ~255 ns |
| function-call (0 args, int result) | ~210 ns | ~376 ns |
| host->guest export call | ~145 ns | ~357 ns |

This confirms the probe reading: compute is identical (in-guest), crossings are
~2-2.5x the native host. The managed host is the shippable integration path; the
C++ host stays in `native/` as the low-crossing-cost fallback.

## Milestone: the host runs client packages through the sandbox

`NanosSharp.Host` now branches the CLIENT package path into the managed `WasmHost`
instead of running packages in process. The trusted host still compiles each
package with Roslyn (the API allow list and `SandboxVerifier` still run outside the
sandbox), producing the same in-memory `AssemblyImage` bytes it always did. What
changes is what happens next: on CLIENT the in-process collectible-ALC execution
(reflection, DI, entity factories, event bus) is skipped - all of that runs inside
the guest instead - and the host only stages the compiled bytes and drives the
reactor.

The pieces:

- `NativeRuntime` exposes `internal static NativeApi* ApiPointer` so the host can
  hand the raw seam pointer to `new WasmHost(api, reactor, stagingDir)`.
- `NanosSharp.Host.csproj` references `NanosSharp.Sandbox` only when
  `NanosSharpSide == Client`, so Wasmtime never enters the server build.
- `ClientSandbox` (CLIENT-only) owns a single `WasmHost`. It resolves the reactor
  module (`NANOSSHARP_REACTOR`, else `<runtimeDir>/nanossharp-reactor.wasm`) and a
  staging directory (`<runtimeDir>/wasm-sandbox`), writes each package's compiled
  assemblies flat into that directory as `<Name>.dll` (the guest's package loader
  globs `/package/*.dll` non-recursively; assembly names are package-prefixed so
  they never collide), and drives the lifecycle. Staging, unstaging and engine
  attach set a dirty flag; the per-tick pump rebuilds the reactor once when dirty
  (so a burst of package loads at join becomes one boot) and otherwise ticks it.
- `PackageManager` routes the CLIENT `OnPackageLoaded` into `ClientSandbox.Stage`,
  ticks the sandbox, and forwards engine attach/detach and unload.

Verified: CLIENT host builds clean (and pulls Wasmtime), server host and the
default solution build clean (no Wasmtime), host unit tests pass. The `WasmHost`
drive path itself is already proven by the harness. What is not yet done, and can
only partly be tested off the real client:

- **Reactor delivery.** The build/CI/installer must ship
  `nanossharp-reactor.wasm` next to the client runtime directory. Until then
  `ClientSandbox` logs that the reactor is missing and client C# packages do not
  run (it does not crash).
- **Guest boot order and incremental load.** Done: `nanos_boot` no longer loads
  packages; a new `nanos_load_packages` export does, so the host calls it after
  `nanos_engine_attached` (the schema is loaded first). The export is incremental -
  it tracks the assemblies it has already loaded and skips them - so a package that
  arrives after boot loads into the running reactor without a reboot. On the host,
  `ClientSandbox` compiles the reactor `Module` once and caches it, boots a single
  reactor, loads new packages incrementally, and reboots (reusing the cached
  module, so only the store and instance are rebuilt) only to recover from a
  failed reactor call. Verified through the harness: `wasm runtime ready` now
  precedes package loading.

## Milestone: single-package load, unload and reload without a reboot

The guest now runs a real per-package host (`WasmPackageHost`) instead of a flat
loader. The host stages each package into its own subdirectory of `/package` with
a generation marker file, and a single `nanos_sync_packages` export reconciles the
loaded set against the staged set: it loads new subdirectories, unloads ones that
disappeared, and reloads ones whose generation changed - all in one call, with no
reactor reboot. Each package gets its own collectible `AssemblyLoadContext` and a
`PackageScope`, and its entry types run inside `PackageContext.Enter(scope)`, so a
package's subscriptions, timers, exports and function handles are tracked against
its scope. Unloading one package stops its entries, disposes its scope (the same
`ReleaseScope` teardown the server's `ManagedPackage.Stop` uses), unloads its load
context, and leaves the other packages running. Cross-package references resolve
through a `Resolving` handler that searches the other packages' load contexts;
references to the runtime's own `NanosSharp` fall back to the default context.

On the host, `ClientSandbox` writes each package into `wasm-sandbox/<name>/` with a
`.gen` file, boots one reactor, and calls `nanos_sync_packages` whenever the staged
set changes - so staging, unstaging and replacing a package never reboot the
reactor.

Verified through the harness against the real reactor: staging the package loads
and starts it, deleting its subdirectory and syncing runs its `Stop` and unloads
it, and staging it again with a new generation reloads and restarts it - all
without rebooting. A caveat found by probe: Mono-wasi does not currently reclaim a
collectible load context's memory on `Unload` (the weak reference stays alive after
forced GC), so an unload is logical - the package stops and can be reloaded, but
its assembly memory is not freed until the reactor is rebooted (the cached-module
reboot remains the memory-reclaim path). Still separate: running the full `Package`
base lifecycle (DI, event bus, the package scripting entity) inside the guest,
which needs the engine's per-package handle crossed into the sandbox.
- **In-game Windows/Proton run.** The native client is Windows-only; Wasmtime.NET
  ships the win-x64 native library, so no extra native wiring is expected, but the
  end-to-end path is untested under Proton.
- **Allow-list removal.** Done: with the sandbox as the real capability boundary,
  the compile-time IL allow list was removed. `ScriptCompiler` no longer takes an
  allow list, `SandboxVerifier`, `ApiAllowList` and `client-allowlist.txt` are
  gone, and client packages now compile with the same framework references and
  `unsafe` support as the server - the sandbox, not the compiler, is what contains
  them. `unsafe` and `[DllImport]` are neutralized by the sandbox anyway (pointers
  reach only the guest's own linear memory; a P/Invoke resolves only to the
  brokered `nanos` imports). Capability hints at build time, if ever wanted, would
  be a separate curated analyzer rather than the old deny list.

## Milestone: the full package lifecycle runs in the sandbox

The guest no longer runs a reflection-only `Start()` stub - `WasmPackageHost` runs
the same lifecycle the server's `ManagedPackage` does, ported into the guest: it
discovers `Package` subclasses, entity factories, exports and services; builds a
`ServiceContainer`, resolves `[Inject]` dependencies, registers event handlers,
and starts (`container.StartAsync` → `OnStart`); and on unload reverses it
(`OnStop`, `container.Stop`, dispose subscriptions, `ReleaseScope` on exports /
timers / functions / entities). The dependency-map investigation confirmed the
managed core runs with `PackageScope.Handle == 0` and `Environment == 0`: seam
calls fall back to `PackageContext.GlobalEnvironment` (set when the engine
attaches), and only the lazy scripting-entity conveniences (`Package.Title` etc.)
need a handle, resolving by name once entities are present. Verified through the
harness: a real `Package` subclass loads, its `[Inject]` service is resolved and
used in `OnStart`, and unload/reload runs `OnStop` and restarts cleanly.

## Milestone: the callback direction (events and entities)

The import direction (guest → engine) was complete; this adds the mirror. The
engine only knows the trusted host's `ManagedCallbacks`, but the function and
entity registrations live in the guest, so the host relays them in. Guest exports
`nanos_invoke_function`, `nanos_release_function`, `nanos_entity_created` and
`nanos_entity_destroyed` dispatch to the guest's `FunctionRegistry` /
`EntityRegistry`. `WasmHost` drives them, marshalling the other way from the import
path: it writes the engine's `NativeArgument` records into guest linear memory
(allocated through the guest's `malloc`, strings copied in, entities interned to
table indices), calls the export, and takes the returned value handle back out of
the value table as the native result. On the trusted host, `NativeCallbacks` gained
CLIENT-only hooks that `ClientSandbox` sets to forward `invoke-function`,
`release-function` and the entity callbacks to the running `WasmHost` instead of
the host's own (empty) registries. Verified through the harness: driving engine
attach subscribes `Client.Tick` inside the guest, and a simulated engine callback
(`WasmHost.InvokeFunction` with a `double` argument) runs the guest handler with
the right value. Full coverage of every argument type, entity propagation and
value lifetime can only be exercised against the real engine (see ship readiness).

## Ship readiness

Done and validated off the real client (builds, host tests, and the mock-engine
harness against the real reactor): the managed Wasmtime.NET host; the full import
seam; package compile → stage → boot; the full package lifecycle (entries, DI,
factories, exports, events) in the sandbox; single-package load / unload / reload
without a reboot; the callback relay; the allow-list removal; and reactor delivery
(`xmake client` builds `nanossharp-reactor.wasm` into the client runtime directory,
and the installer and self-update ship and swap it wholesale).

The remaining gate needs the real Windows / Proton client, which cannot run on the
build host. In-game checklist:

1. `xmake client -r <version>` (with the WASI SDK installed) and install the bundle
   into a game client; confirm `nanossharp-reactor.wasm` sits next to the runtime.
2. Join a server that pushes a C# client package; confirm the reactor boots, the
   package loads, `OnStart` runs, and `[Inject]`/DI resolve.
3. Fire a client event the package subscribes to; confirm the handler runs (the
   full callback relay with real arguments and entity arguments).
4. Confirm entities created by the engine reach the package (entity-created relay)
   and that `Package.Title`/`Version` resolve (the `Package_S` scripting entity).
5. Hot-reload: have the server replace the package; confirm it unloads and reloads
   without disconnecting, and that other packages keep running.

Known caveats to validate or accept in-game: Mono-wasi does not reclaim a
collectible load context's memory on unload (logical unload only; reboot reclaims);
the reactor runs interpreted (Lua-comparable speed; the AOT speed-up is still gated
on the AOT-reactor-init problem); and the callback relay's argument/lifetime
coverage beyond the tested `double` path is by construction, not yet measured.

## Non-goals / unchanged

- Server runtime stays in-process CoreCLR (the server operator is trusted; full
  performance, no sandbox).
- The package-facing C# API (DI, attributes, `Package` entry types, hot reload)
  stays as-is. That is the whole point of choosing the runtime-in-wasm model.
