using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class Event : AuthorityOwned
{
    [JsonProperty("arguments")]
    internal Value[] Arguments { get; set; }
}