# NanosSharp

The .NET scripting API for [nanos world](https://nanos-world.com) servers. Reference this package to write server
packages in C# with full IDE support, the NanosSharp runtime on the server provides the assembly at runtime.

```csharp
public sealed class Gamemode : Package
{
    protected override void OnStart() => Player.OnReady += player => player.SendMessage("Hello from C#!");
}
```

- Typed API generated from the nanos world server and its API definitions
- Typed events, `[ServerEvent]` handlers and overridable entity hooks
- `async`/`await` on the main thread, Lua interop through `Exports`

Create a package with the templates:

```sh
dotnet new install NanosSharp.Templates
dotnet new nanossharp-script -n MyGamemode
```

Documentation: https://dasdarki.de/nanossharp/
