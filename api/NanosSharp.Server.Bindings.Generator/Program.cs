using NanosSharp.Server.Bindings.Generator;

var outputPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "NanosSharp.Server.Bindings"));
var generator = new BindingsGenerator(outputPath);
generator.PreInitialize();
generator.Initialize();
generator.Execute();