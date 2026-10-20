# NanosSharp Templates

`dotnet new` templates for [NanosSharp](https://nanossharp.dasdarki.de) packages on nanos world servers.

```sh
dotnet new install NanosSharp.Templates

# C# the server compiles and hot reloads, with a server, a client and a shared side
dotnet new nanossharp-script -n MyGamemode -o Packages/my-gamemode

# a package compiled into an assembly, built into its Server folder
dotnet new nanossharp-assembly -n MyGamemode -o Packages/my-gamemode
```

Use a valid C# identifier as the name, it becomes the class name of the package entry point.

## Script package layout

```
my-gamemode/
  .gitignore
  Package.toml              the nanos world package manifest
  MyGamemode.slnx           opens the server and the client project together
  Server/
    MyGamemode.Server.csproj
    MyGamemode.cs           the server entry point
  Client/
    MyGamemode.Client.csproj
    MyGamemode.cs           the client entry point, sandboxed by the server
  Shared/
    Shared.cs               compiled into both sides
```

The server compiles `Server` plus `Shared`, the client compiles `Client` plus `Shared`, each into its own
assembly. There is one project per side, `Shared` has no project of its own but is linked into both, so it gets
full IntelliSense on either side. `SERVER` is defined for the server project and `CLIENT` for the client one, so
`#if SERVER` and `#if CLIENT` in shared code resolve the same way the server compiles them. Open the shared file
in both projects through the solution to switch between the two.

The `.csproj` files are only for the IDE and for NuGet `PackageReference`s, the server compiles the sources
itself. Drop the `Client` folder if the package has no client code, or the `Server` folder if it has no server
code.
