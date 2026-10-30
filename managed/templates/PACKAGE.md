# NanosSharp Templates

`dotnet new` templates for [NanosSharp](https://nanossharp.dasdarki.de) packages on nanos world servers.

```sh
dotnet new install NanosSharp.Templates

# C# the server compiles and hot reloads, with a server, a client and a shared side
dotnet new nanossharp-script -n MyGamemode -o Packages/my-gamemode

# a package compiled into an assembly, built into its Server folder
dotnet new nanossharp-assembly -n MyGamemode -o Packages/my-gamemode

# the same, but built into its Client folder and run inside the sandbox
dotnet new nanossharp-assembly -n MyGamemode -o Packages/my-gamemode --side Client
```

Use a valid C# identifier as the name, it becomes the class name of the package entry point.

## Source or prebuilt assemblies

Each side can be delivered either as source or as a prebuilt assembly, independently:

- Drop `*.cs` into `Server/`, `Client/` or `Shared/` and NanosSharp compiles them for you. The server compiles on the
  server and hot reloads on change; the client compiles on each player's machine, inside the trusted host. This is the
  default and needs no build toolchain of your own.
- Drop a prebuilt `*.dll` (plus any dependency assemblies it needs) into `Server/` or `Client/` and NanosSharp loads it
  as is, with no compilation. Reference `NanosSharp` for a server assembly and `NanosSharp.Client` for a client assembly,
  with `ExcludeAssets="runtime"` so the API assembly itself is not copied. Use this for closed-source packages or to skip
  per-player compilation of client code.

Both work the same at runtime: a server assembly runs in process with full trust, and a client assembly runs inside the
WebAssembly sandbox with no ambient authority, exactly like compiled client source. A side may mix both; sources are then
compiled against the prebuilt assemblies in the same folder.

## Calling other C# packages

One C# package can call another's public API directly, with full type safety, instead of going through the untyped Lua
`Exports`. Declare the dependency in `Package.toml` and resolve the API by its contract:

```toml
packages_requirements = ["economy"]   # hard, must be present
packages_optional = ["bonus"]         # soft, used only when present
```

The provider publishes an implementation, by attribute or explicitly:

```csharp
public interface IEconomyApi { void AddMoney(Player player, long amount); }

[PackageApi]                                        // published under IEconomyApi and the concrete type
public sealed class EconomyApi : IEconomyApi { /* ... */ }
// or from OnStart: PackageApi.Provide<IEconomyApi>(new EconomyApi());
```

The consumer resolves it by contract:

```csharp
var economy = PackageApi.Require<IEconomyApi>();     // throws when absent
PackageApi.TryGet<IBonusApi>(out var bonus);         // for optional dependencies

[Inject] private IEconomyApi _economy = null!;       // or injected like any service
[Inject] private PackageApi<IEconomyApi> _economy;   // a handle that survives provider reloads
```

Changing a required package reloads its dependents, so a resolved instance stays valid. Prefer a `PackageApi<T>` handle
(or re resolve through `PackageApi.Get`) when the provider should hot reload without restarting the consumer, and keep
the contract in a small library package both sides require so its type identity stays stable. This works the same on the
server and inside the client sandbox. `Exports` stays the way to talk to Lua packages and for loose, dynamic coupling.

## Script package layout

```
my-gamemode/
  .gitignore
  Package.toml              the nanos world package manifest
  MyGamemode.slnx           opens all three projects together
  Server/
    MyGamemode.Server.csproj
    MyGamemode.cs           the server entry point
  Client/
    MyGamemode.Client.csproj
    MyGamemode.cs           the client entry point, sandboxed by the server
  Shared/
    MyGamemode.Shared.csproj
    Shared.cs               compiled into both sides
```

The server compiles `Server` plus `Shared`, the client compiles `Client` plus `Shared`, each into its own
assembly. `Shared` is its own project that `Server` and `Client` reference, so shared code has one home and one
namespace instead of appearing twice. Every project uses the `MyGamemode` root namespace, so types from any side
are reachable without extra `using`s.

`SERVER` is defined for the server project and `CLIENT` for the client one. Those symbols apply to side code;
shared code is side neutral, put anything server or client specific in `Server` or `Client`. The `.csproj` files
are only for the IDE and for NuGet `PackageReference`s, the server compiles the sources itself. Drop the `Client`
folder if the package has no client code, or the `Server` folder if it has no server code.
