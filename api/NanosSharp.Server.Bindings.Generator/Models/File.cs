using Newtonsoft.Json.Linq;

namespace NanosSharp.Server.Bindings.Generator.Model;

/// <summary>
/// The model which describes one file and their data.
/// </summary>
internal class File
{
    /// <summary>
    /// The path to the file.
    /// </summary>
    internal string Path { get; }
    
    /// <summary>
    /// The data of the file.
    /// </summary>
    internal JObject Data { get; }

    internal File(string path, JObject data)
    {
        Path = path;
        Data = data;
    }
}