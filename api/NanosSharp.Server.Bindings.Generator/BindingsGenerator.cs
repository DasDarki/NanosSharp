using System.Text;
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
    private readonly List<string> _enumNames = new();
    
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
        var enumFile = _files.FirstOrDefault(f => f.Path.StartsWith("Enums"));
        if (enumFile != null)
        {
            GenerateEnums(enumFile.Data.ToObject<Model.Enum>()!);
        }
        
        foreach (var file in _files.Where(file => !file.Path.StartsWith("Enums")))
        {
            if (file.Path.StartsWith("Utility") || file.Path.StartsWith("StandardLib"))
            {
                continue;
            }
            
            GenerateClass(Path.GetDirectoryName(file.Path)!, file.Data.ToObject<Class>()!);
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
            if (clazz.Functions != null)
            {
                GenerateFunctions(clazz.Functions, false);
            }
            
            if (clazz.StaticFunctions != null)
            {
                GenerateFunctions(clazz.StaticFunctions, true);
            }

            return;

            void GenerateFunctions(IEnumerable<Function> functions, bool isStatic)
            {
                var usedFunctions = new List<string>();
                
                foreach (var func in functions)
                {
                    var funcId = GetFunctionIdentifier(func);
                    if (usedFunctions.Contains(funcId)) continue;
                    usedFunctions.Add(funcId);
                    
                    if (func is not {IsAllowed: true}) continue;
                    if (func.Return != null && func.Return.Any(r => r.Type == "iterator")) continue;

                    if (func.Parameters != null)
                    {
                        for (var i = 0; i < func.Parameters.Length; i++)
                        {
                            var param = func.Parameters[i];
                            if (param.Type.ToLower() == "table" && param.TableProperties != null)
                            {
                                classBuilder.AddStruct(func.Name + "_Param" + i, structBuilder =>
                                {
                                    foreach (var tableProperty in param.TableProperties)
                                    {
                                        structBuilder.AddField(tableProperty.Name, TypeTransformer.Transform(tableProperty.Type, _enumNames));
                                    }
                                });
                            }
                        }
                    }

                    if (func.Return != null)
                    {
                        for (var i = 0; i < func.Return.Length; i++)
                        {
                            var ret = func.Return[i];
                            if (ret.Type.ToLower() == "table" && ret.TableProperties != null)
                            {
                                classBuilder.AddStruct(func.Name + "_Return" + i, structBuilder =>
                                {
                                    foreach (var tableProperty in ret.TableProperties)
                                    {
                                        structBuilder.AddField(tableProperty.Name, TypeTransformer.Transform(tableProperty.Type, _enumNames));
                                    }
                                });
                            }
                        }
                    }
                    
                    classBuilder.AddFunction(isStatic, func.Name, funcBuilder =>
                    {
                        if (func.Parameters != null)
                        {
                            for (var i = 0; i < func.Parameters.Length; i++)
                            {
                                var param = func.Parameters[i];
                                var isNullable = param.Type.EndsWith("?") || param.Default != null;
                                if (param.Type.ToLower() == "table" && param.TableProperties != null)
                                {
                                    funcBuilder.Param(func.Name + "_Param" + i + (isNullable ? "?" : ""), ISourceBuilder.MakeSafeName(param.Name) + (isNullable ? " = null" : ""));
                                }
                                else
                                {
                                    var isVararg = param.Name.EndsWith("...");
                                    var transformed = (isVararg ? "params " : "") + TypeTransformer.Transform(param.Type, _enumNames) + (isVararg ? "[]" : "");
                                    funcBuilder.Param(transformed + (isNullable && !transformed.EndsWith("?") && !isVararg ? "?" : ""), ISourceBuilder.MakeSafeName(param.VararglessName) + (isNullable && !isVararg ? " = null" : ""));
                                }
                            }
                        }

                        if (func.Return != null)
                        {
                            for (var i = 0; i < func.Return.Length; i++)
                            {
                                var ret = func.Return[i];
                                if (ret.Type.ToLower() == "table" && ret.TableProperties != null)
                                {
                                    funcBuilder.Returns(func.Name + "_Return" + i);
                                }
                                else
                                {
                                    funcBuilder.Returns(TypeTransformer.Transform(ret.Type, _enumNames));
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
                                        var push = TypeTransformer.DeterminePush(param.Type, _enumNames);
                                        var isVararg = param.Name.EndsWith("...");
                                        
                                        bool NeedsValue()
                                        {
                                            return isNullable && !isVararg && !param.Type.ToLower().Contains("path") &&
                                                   !param.Type.ToLower().StartsWith("string") && !param.Type.ToLower().StartsWith("any") &&
                                                   !param.Type.ToLower().StartsWith("function") &&
                                                   !param.Type.ToLower().StartsWith("table");
                                        }

                                        if (isVararg)
                                        {
                                            body.Add((intend ? "     " : "") + "foreach (var a in " + ISourceBuilder.MakeSafeName(param.VararglessName) + ") {");
                                            body.Add((intend ? "     " : "") + "    pc++;");
                                            body.Add((intend ? "     " : "") + "    vm." + push.Replace("%", "a" + (NeedsValue() ? ".Value" : "")) + ";");
                                            body.Add((intend ? "     " : "") + "}");
                                        }
                                        else
                                        {
                                            body.Add((intend ? "     " : "") + "pc++;");
                                            body.Add((intend ? "     " : "") + "vm." + push.Replace("%", ISourceBuilder.MakeSafeName(param.VararglessName) + (NeedsValue() ? ".Value" : "")) + ";");
                                        }
                                    }
                                    
                                    var isNullable = (param.Type.EndsWith("?") || param.Default != null) && !param.Name.EndsWith("...");
                                    if (isNullable)
                                    {
                                        body.Add("if (" + ISourceBuilder.MakeSafeName(param.VararglessName) + " != null)");
                                        body.Add("{");
                                        AddPushToBody(true, true);
                                        body.Add("}");
                                    }
                                    else
                                    {
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
                                    var pop = TypeTransformer.DeterminePull(ret.Type, _enumNames, out var needPop);
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
            _enumNames.Add(@enum.Key);
            
            builder.AddEnum(@enum.Key, enumBuilder =>
            {
                enumBuilder.SetDescription(@enum.Value.Description);
                
                foreach (var value in @enum.Value.Enums)
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

    private string GetFunctionIdentifier(Function f)
    {
        var sb = new StringBuilder();

        if (f.Return != null)
        {
            foreach (var v in f.Return)
            {
                sb.Append(v.Type);
            }
        }

        sb.Append(f.Name);
        
        if (f.Parameters != null)
        {
            foreach (var v in f.Parameters)
            {
                sb.Append(v.Type);
            }
        }
        
        return sb.ToString();
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