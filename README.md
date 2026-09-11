# NanosSharp

.NET runtime for [nanos world](https://nanos-world.com) servers. Write server packages in C# next to (or instead of) Lua.

> Early development. Linux dedicated server only.

## How it works

```
NanosWorldServer
 └─ libnanossharp.so  (LD_PRELOAD, C++)
     ├─ hooks scripting engine lifecycle, server tick and package load/unload
     ├─ talks to the engine-agnostic reflection layer (IClass/IFunction/ScriptValue) instead of the Lua stack
     └─ hosts CoreCLR via hostfxr
         └─ NanosSharp.Host (C#)
             ├─ loads every package with C# code in Packages/<name>/Server
             │   into its own collectible AssemblyLoadContext and the package's own scripting environment
             ├─ compiles *.cs files with Roslyn, loads *.dll files directly
             └─ hot reloads a package when its sources change
```

Every C# package is a regular nanos world package with its own `Package.toml`, environment and entry in `Server.GetPackages()`.

## Building

Requirements: xmake 3, GCC or Clang with C++20, .NET 10 SDK.

```sh
xmake                              # native library
dotnet build managed/NanosSharp.slnx
xmake deploy -s /path/to/server    # build everything and install runtime and examples into a server
```

Start the server with `./NanosSharp.sh` instead of `./NanosWorldServer.sh`.

## Writing a package

```
Packages/my-package/
 ├─ Package.toml
 └─ Server/
     └─ Main.cs
```

```csharp
public sealed class MyPackage : Package
{
    protected override void OnStart()
    {
        Log.Info($"Running on {Server.Map}");
        Player.Ready += player => player.Possess(Character.Spawn(new Vector(0, 0, 200), Rotator.Zero));
    }
}
```

Compiled assemblies placed in `Server/` work as well, see `managed/examples`.
Calls into nanos world must happen on the main thread, use `MainThread.Post` or `await` from main thread code.
Everything without a typed wrapper yet is reachable through `ScriptApi` and `Entity.Invoke`.

## Layout

| Path | Content |
|---|---|
| `native/src/game` | Reverse engineered engine types and symbols |
| `native/src/hooks` | Inline hooking |
| `native/src/host` | CoreCLR bootstrap |
| `native/src/bridge` | C ABI between native and managed |
| `managed/NanosSharp` | Public API (`Core`, `Entities`, `Logging`, `Mathematics`, `Packages`, `Scripting`, `Threading`) and internal `Interop` |
| `managed/NanosSharp.Host` | Package loading, Roslyn compilation, hot reload |
| `managed/examples` | Example packages |
