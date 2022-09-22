using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class Document
{
    [JsonProperty("name")]
    internal string Name { get; set; }
    
    [JsonProperty("description")]
    internal string Description { get; set; }
    
    [JsonProperty("description_long")]
    internal string DescriptionLong { get; set; }
}