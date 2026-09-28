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

The API mirrors Lua and adds C# conveniences on top:

```csharp
var prop = new Prop(location, rotation, "nanos-world::SM_Cube");   // like Prop(...) in Lua
prop.SetScale(new Vector(2, 2, 2));                                  // Lua style
prop.Scale = new Vector(2, 2, 2);                                    // C# property, same call
Character.OnDeath += (character, damage, bone, type, direction, instigator, causer) => { };
character.SubscribeDeath(...);                                       // events of one entity
class ExplosiveBarrel(Vector location) : Prop(location, Rotator.Zero, "nanos-world::SM_Cube");
```

Typed event classes with automatic registration, in the style of Bukkit:

```csharp
public sealed class Gameplay : Package
{
    [ServerEvent]
    private void OnDeath(CharacterDeathEvent e) => Log.Info($"{e.Character} was killed by {e.Instigator}");

    [ServerEvent(Priority = -10)]
    private void OnChat(ChatPlayerSubmitEvent e)
    {
        if (e.Message?.Contains("spam") == true)
        {
            e.Cancel();
        }
    }
}
```

Handlers on package entry classes are registered automatically, other objects through `EventBus.Register(listener)`.

Own entity classes, every entity of that scripting class is wrapped with them (also existing ones):

```csharp
[EntityFactory]
public class RoleplayPlayer : Player
{
    public int Money { get; set; }
}
```

Compiled assemblies placed in `Server/` work as well, see `managed/examples`.
Publish such projects into `Server/`, libraries, `*.deps.json` and native libraries next to them are resolved automatically.
A package listed in `packages_requirements` shares its assemblies, so dependents can use its types.

### NuGet and IDE support

A package with `.cs` files gets a generated `Server/<package>.csproj` on first load. Open it in Rider or VS Code for
IntelliSense, it references the server's `NanosSharp.dll` and uses the same global usings as the server compiler.
`PackageReference` items in that project are downloaded by the server itself (no .NET SDK needed on the server),
including transitive dependencies and native libraries, and cached in `NanosSharp/cache/nuget`.

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.13" />
</ItemGroup>
```

Every package gets its own copy of its libraries, so packages can use different versions of the same library.

## Hot reload

Changing a `.cs` or `.dll` file in `Server/` rebuilds the package first. If the build fails, the running version stays active.
Otherwise the package is reloaded through nanos world itself, followed by every C# package that requires it.
State is not preserved, so keep it in values, databases or packages that are not reloaded.
After a reload the previous assemblies are checked to be unloaded, a warning names packages that still hold references
(running tasks, timers, threads or static handlers outside of nanos world).

## API generation

The typed API in `managed/NanosSharp/Generated` is generated, never edit it by hand.

```sh
xmake dump-api    # dump the native scripting API of the server into the generator data
xmake generate    # combine it with github.com/nanos-world/api at the pinned revision
```

Native signatures decide types and availability, the API definitions provide names, documentation, events and enums.
The output is deterministic, `build/generator/report.md` lists everything that could not be mapped.

## Performance

`xmake test -d benchmarks` runs the same operations from C# and Lua inside the server (20 000 calls each):

| Operation | Lua | C# |
|---|---|---|
| Static getter (`Server.GetTickRate`) | ~50 ns | ~150 ns |
| Entity getter (`GetLocation`) | ~260 ns | ~290 ns |
| Entity setter (`SetLocation`) | ~250 ns | ~340 ns |
| Event roundtrip (`Events.Call` with one handler) | ~270 ns | ~820 ns |

Arguments are passed as blittable structs without native allocations, callbacks use compiled invokers.

## Configuration

`NanosSharp.toml` in the server directory (created on first start):

```toml
[hot_reload]
enabled = true
delay_milliseconds = 500
verify_unload = true

[nuget]
sources = ["https://api.nuget.org/v3/index.json"]

[projects]
generate = true
```

## Debugging

The server hosts a regular CoreCLR, so attaching a .NET debugger (Rider, VS Code with netcoredbg) to the
`NanosWorldServer` process is the intended workflow. Assemblies are loaded together with their symbols.
This has not been verified yet.

## Testing

`xmake test` deploys everything into the server, starts it isolated on port 7797 with the packages in `tests/`
and reports the results of the in-server test suite.
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
| `managed/NanosSharp/Generated` | Generated API (do not edit) |
| `managed/tools/NanosSharp.Generator` | API generator and native API dump |
| `managed/examples` | Example packages |
| `tests` | In-server integration test packages |
