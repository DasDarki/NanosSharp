using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class Function : AuthorityOwned
{
    [JsonProperty("parameters")]
    internal Value[] Parameters { get; set; }
    
    [JsonProperty("return")]
    internal Value[] Return { get; set; }
}