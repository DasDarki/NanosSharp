using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model
{
    internal class EnumValue
    {
        [JsonProperty("key")]
        internal string Key { get; set; }
        
        [JsonProperty("value")]
        internal string Value { get; set; }
    }
}