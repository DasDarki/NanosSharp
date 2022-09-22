using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

public class EnumValue
{
    [JsonProperty("key")]
    internal string Key { get; set; }
        
    [JsonProperty("value")]
    internal string Value { get; set; }
    
    [JsonProperty("description")]
    internal string Description { get; set; }
}