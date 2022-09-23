using LibGit2Sharp;
using NanosSharp.Server.Bindings.Generator.Model;
using NanosSharp.Server.Bindings.Generator.SourceBuilder;
using Newtonsoft.Json.Linq;
using File = System.IO.File;

namespace NanosSharp.Server.Bindings.Generator;

/// <summary>
/// The bindings generator fetches the API from the JSON repository of nanos world and generates the newest C#
/// bindings.
/// </summary>
internal class BindingsGenerator
{
    private readonly bool _useBleedingEdge = true;
    private readonly List<Model.File> _files = new();
    
    private readonly string _outputPath;
    private readonly string _repositoryPath;

    internal BindingsGenerator(string outputPath)
    {
        _outputPath = outputPath;
        _repositoryPath = Path.Combine(Environment.CurrentDirectory, "nanos-world-api");
    }

    /// <summary>
    /// Gets called before the initializing and takes care of cloning or updating the repository
    /// </summary>
    public void PreInitialize()
    {
        foreach (var file in Directory.GetFiles(_outputPath, "*.cs", SearchOption.AllDirectories))
        {
            if (file.Contains("bin") || file.Contains("obj")) continue;
            File.Delete(file);
        }
        
        Repository.Clone("https://github.com/nanos-world/api.git", _repositoryPath);
    }

    /// <summary>
    /// Gets called before the generation process and takes care of loading the newest JSON repository data.
    /// </summary>
    public void Initialize()
    {
        void IterateDirectory(string dirPath)
        {
            foreach (var dir in Directory.GetDirectories(dirPath))
            {
                IterateDirectory(dir);
            }
            
            foreach (var file in Directory.GetFiles(dirPath))
            {
                if (file.EndsWith(".json"))
                {
                    var path = file.Replace(_repositoryPath, string.Empty).TrimStart(Path.DirectorySeparatorChar);
                    if (path.ToLower().StartsWith("stable")) continue;
                    
                    var json = JObject.Parse(File.ReadAllText(file));
                    var fileModel = new Model.File(path, json);
                    _files.Add(fileModel);
                }
            }
        }

        IterateDirectory(_useBleedingEdge ? _repositoryPath : Path.Combine(_repositoryPath, "Stable"));
        DeleteReadOnlyDirectory(_repositoryPath);
    }

    /// <summary>
    /// Gets called after the initialization and generates the C# bindings.
    /// </summary>
    public void Execute()
    {
        foreach (var file in _files)
        {
            if (file.Path.StartsWith("Enums"))
            {
                GenerateEnums(file.Data.ToObject<Model.Enum>()!);
            }
            else
            {
                GenerateClass(Path.GetDirectoryName(file.Path)!, file.Data.ToObject<Class>()!);
            }
        }
    }

    /// <summary>
    /// Generates the C# bindings for the given class.
    /// </summary>
    private void GenerateClass(string dir, Class clazz)
    {
        if (!clazz.IsAllowed) return;
        
        var builder = new CSharpFileBuilder().WithUsings();
        
        builder.AddClass(dir.StartsWith("Static") || dir.StartsWith("Utility"), clazz.Name, clazz.Inheritance ?? Array.Empty<string>(), classBuilder =>
        {
            void GenerateFunctions(IEnumerable<Function> functions, bool isStatic)
            {
                foreach (var func in functions)
                {
                    if (func is not {IsAllowed: true}) continue;
                    if (func.Return != null && func.Return.Any(r => r.Type == "iterator")) continue;

                    if (func.Parameters != null)
                    {
                        for (int i = 0; i < func.Parameters.Length; i++)
                        {
                            var param = func.Parameters[i];
                            if (param.Type.ToLower() == "table" && param.TableProperties != null)
                            {
                                classBuilder.AddStruct(func.Name + "_Param" + i, structBuilder =>
                                {
                                    foreach (var tableProperty in param.TableProperties)
                                    {
                                        structBuilder.AddField(tableProperty.Name, tableProperty.Type);
                                    }
                                });
                            }
                        }
                    }

                    if (func.Return != null)
                    {
                        for (int i = 0; i < func.Return.Length; i++)
                        {
                            var ret = func.Return[i];
                            if (ret.Type.ToLower() == "table" && ret.TableProperties != null)
                            {
                                classBuilder.AddStruct(func.Name + "_Return" + i, structBuilder =>
                                {
                                    foreach (var tableProperty in ret.TableProperties)
                                    {
                                        structBuilder.AddField(tableProperty.Name, TypeTransformer.Transform(tableProperty.Type));
                                    }
                                });
                            }
                        }
                    }
                    
                    classBuilder.AddFunction(isStatic, func.Name, funcBuilder =>
                    {
                        if (func.Parameters != null)
                        {
                            for (int i = 0; i < func.Parameters.Length; i++)
                            {
                                var param = func.Parameters[i];
                                var isNullable = param.Type.EndsWith("?") || param.Default != null;
                                if (param.Type.ToLower() == "table" && param.TableProperties != null)
                                {
                                    funcBuilder.Param(func.Name + "_Param" + i + (isNullable ? "?" : ""), param.Name + (isNullable ? " = null" : ""));
                                }
                                else
                                {
                                    var isVararg = param.Name.EndsWith("...");
                                    var transformed = (isVararg ? "params " : "") + TypeTransformer.Transform(param.Type) + (isVararg ? "[]" : "");
                                    funcBuilder.Param(transformed + (isNullable && !transformed.EndsWith("?") && !isVararg ? "?" : ""), param.VararglessName + (isNullable && !isVararg ? " = null" : ""));
                                }
                            }
                        }

                        if (func.Return != null)
                        {
                            for (int i = 0; i < func.Return.Length; i++)
                            {
                                var ret = func.Return[i];
                                if (ret.Type.ToLower() == "table" && ret.TableProperties != null)
                                {
                                    funcBuilder.Returns(func.Name + "_Return" + i);
                                }
                                else
                                {
                                    funcBuilder.Returns(TypeTransformer.Transform(ret.Type));
                                }
                            }
                        }

                        funcBuilder.Body(body =>
                        {
                            body.Add("int pc = 0;");
                            body.Add("vm.PushGlobalTable();");
                            body.Add("vm.GetField(-1, \"" + clazz.Name + "\");");

                            if (!dir.StartsWith("Struct") && !isStatic)
                            {
                                body.Add("vm.GetField(-1, \"__function\");");
                            }
                            
                            body.Add("vm.GetField(-1, \"" + func.Name + "\");");

                            if (!isStatic)
                            {
                                body.Add("pc++;");
                                body.Add("vm.RawGetI(ILuaVM.RegistryIndex, selfRef);");
                            }

                            if (func.Parameters != null)
                            {
                                foreach (var param in func.Parameters)
                                {
                                    void AddPushToBody(bool intend = false, bool isNullable = false)
                                    {
                                        if (param.Type.Contains("[]"))
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("Warning: " + clazz.Name + "." + func.Name + " has an array parameter. This is not supported by the C# bindings.");
                                            Console.ResetColor();
                                            return;
                                        }

                                        var push = TypeTransformer.DeterminePush(param.Type);
                                        if (push.StartsWith("Dictionary"))
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("Warning: " + clazz.Name + "." + func.Name + " has a table parameter. This is not supported by the C# bindings.");
                                            Console.ResetColor();
                                            return;
                                        }
                                        
                                        var isVararg = param.Name.EndsWith("...");
                                        body.Add((intend ? "     " : "") + "vm." + push.Replace("%", param.VararglessName + (isNullable && !isVararg && param.Type.ToLower() != "string" ? ".Value" : "")) + ";");
                                    }
                                    
                                    var isNullable = (param.Type.EndsWith("?") || param.Default != null) && !param.Name.EndsWith("...");
                                    if (isNullable)
                                    {
                                        body.Add("if (" + param.VararglessName + " != null)");
                                        body.Add("{");
                                        body.Add("     pc++;");
                                        AddPushToBody(true, true);
                                        body.Add("}");
                                    }
                                    else
                                    {
                                        body.Add("pc++;");
                                        AddPushToBody();
                                    }
                                }
                            }
                            
                            var returnCount = func.Return?.Length ?? 0;
                            body.Add("vm.MCall(pc, " + returnCount + ");");
                            
                            if (func.Return != null)
                            {
                                for (int i = returnCount - 1; i >= 0; i--)
                                {
                                    var ret = func.Return[i];
                                    if (ret.Type.Contains("[]"))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine("Warning: " + clazz.Name + "." + func.Name + " has an array parameter. This is not supported by the C# bindings.");
                                        Console.ResetColor();
                                        continue;
                                    }

                                    var pop = TypeTransformer.DeterminePull(ret.Type, out var needPop);
                                    if (pop.StartsWith("Dictionary"))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine("Warning: " + clazz.Name + "." + func.Name + " has a table parameter. This is not supported by the C# bindings.");
                                        Console.ResetColor();
                                        continue;
                                    }
                                    
                                    body.Add($"{(returnCount > 1 ? "" : "var ")}r{i} = vm.{pop};");
                                    
                                    if (needPop)
                                    {
                                        body.Add("vm.Pop();");
                                    }
                                }
                            }
                            
                            body.Add("vm.ClearStack();");
                                
                            if (returnCount == 1)
                            {
                                body.Add("return r0;");
                            }
                        });
                    });
                }
            }
            
            if (clazz.Functions != null)
            {
                GenerateFunctions(clazz.Functions, false);
            }
            
            if (clazz.StaticFunctions != null)
            {
                GenerateFunctions(clazz.StaticFunctions, true);
            }
        });
        
        AddSource(Path.Combine(dir, clazz.Name), builder.Generate());
    }

    /// <summary>
    /// Generates the C# bindings for the given enums.
    /// </summary>
    private void GenerateEnums(Model.Enum enums)
    {
        var builder = new CSharpFileBuilder();
            
        foreach (var @enum in enums)
        {
            builder.AddEnum(@enum.Key, enumBuilder =>
            {
                foreach (var value in @enum.Value)
                {
                    enumBuilder.AddValue(value.Key, value.Value);
                }
            });
        }
            
        AddSource("Enums", builder.Generate());
    }

    private void AddSource(string name, string code)
    {
        var filename = Path.Combine(_outputPath, $"{name}.g.cs");
        Directory.CreateDirectory(Path.GetDirectoryName(filename)!);
        File.WriteAllText(filename, code);
        
        Console.WriteLine("Generated: " + filename);
    }

    private void DeleteReadOnlyDirectory(string directory)
    {
        foreach (var subdirectory in Directory.EnumerateDirectories(directory)) 
        {
            DeleteReadOnlyDirectory(subdirectory);
        }
        
        foreach (var fileName in Directory.EnumerateFiles(directory))
        {
            var fileInfo = new FileInfo(fileName)
            {
                Attributes = FileAttributes.Normal
            };
            fileInfo.Delete();
        }
        
        Directory.Delete(directory);
    }
}