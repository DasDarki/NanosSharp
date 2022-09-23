using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model
{
    internal class TransformedValue
    {
        [JsonProperty("type")]
        internal string Type { get; set; }
    
        [JsonProperty("name")]
        internal string Name { get; set; }
    
        [JsonProperty("default")]
        internal string? Default { get; set; }
    
        [JsonProperty("isVararg")]
        internal bool IsVararg { get; set; }
    
        [JsonProperty("description")]
        internal string Description { get; set; }
    }
}