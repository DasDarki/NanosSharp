using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace NanosSharp.Server.Bindings.Generator;

/// <summary>
/// The bindings generator fetches the API from the JSON repository of nanos world and generates the newest C#
/// bindings.
/// </summary>
[Generator]
public class BindingsGenerator : ISourceGenerator
{
    private readonly bool _useBleedingEdge = true;
    private readonly List<Model.File> _files = new();

    /// <summary>
    /// Gets called before the generation process and takes care of loading the newest JSON repository data.
    /// </summary>
    public void Initialize(GeneratorInitializationContext context)
    {
        var res = GetRequest("https://api.github.com/repos/nanos-world/api/git/trees/6d04f16ac621c1fda2b08093bf327e253078a541?recursive=1");
        var tree = res["tree"]?.ToObject<JArray>();
        if (tree == null)
        {
            Console.WriteLine("Failed to load the JSON repository data.");
            return;
        }

        bool CheckPath(string path)
        {
            return path.EndsWith(".json") && (_useBleedingEdge ? !path.StartsWith("Stable/") : path.StartsWith("Stable/"));
        }

        var filenames = tree.Where(f => f["type"]?.ToString() == "blob" && CheckPath(f["path"]?.ToString()!)).Select(f => f["path"]?.ToString()!).ToList();
        foreach (var filename in filenames)
        {
            var file = GetRequest($"https://raw.githubusercontent.com/nanos-world/api/main/{filename}");
            _files.Add(new Model.File(filename.Replace("Stable/", ""), file));
        }
    }

    /// <summary>
    /// Gets called after the initialization and generates the C# bindings.
    /// </summary>
    public void Execute(GeneratorExecutionContext context)
    {
        
    }

    /// <summary>
    /// Makes a GET request to the given url and returns the response JSON body.
    /// </summary>
    /// <param name="url">The target url.</param>
    /// <returns>The response body.</returns>
    private JObject GetRequest(string url)
    {
        using var client = new HttpClient();
        return JObject.Parse(client.GetStringAsync(url).GetAwaiter().GetResult());
    }
}