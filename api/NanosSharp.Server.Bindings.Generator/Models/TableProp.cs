using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class TableProp
{
    [JsonProperty("name")]
    internal string Name { get; set; }
    
    [JsonProperty("type")]
    internal string Type { get; set; }
}