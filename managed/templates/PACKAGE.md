# NanosSharp Templates

`dotnet new` templates for [NanosSharp](https://dasdarki.de/nanossharp/) packages on nanos world servers.

```sh
dotnet new install NanosSharp.Templates

# C# scripts compiled and hot reloaded by the server
dotnet new nanossharp-script -n MyGamemode -o Packages/my-gamemode

# a compiled assembly built into the Server folder of the package
dotnet new nanossharp-assembly -n MyGamemode -o Packages/my-gamemode
```

Use a valid C# identifier as name, it becomes the class name of the package entry point.
