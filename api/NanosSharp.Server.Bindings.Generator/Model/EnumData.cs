using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class EnumData
{
    [JsonProperty("description")]
    internal string Description { get; set; }
    
    [JsonProperty("enums")]
    internal List<EnumValue> Enums { get; set; }
}