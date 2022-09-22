using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class Value : Document
{
    [JsonProperty("type")]
    internal string Type { get; set; }
    
    [JsonProperty("default")]
    internal string Default { get; set; }
    
    [JsonProperty("table_properties")]
    internal TableProp[]? TableProperties { get; set; }
}