using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class Class : AuthorityOwned
{
    [JsonProperty("constructor")]
    internal Value[]? Constructor { get; set; }
    
    [JsonProperty("properties")]
    internal Value[] Properties { get; set; }
    
    [JsonProperty("functions")]
    internal Function[] Functions { get; set; }
    
    [JsonProperty("static_functions")]
    internal Function[] StaticFunctions { get; set; }
    
    [JsonProperty("events")]
    internal Event[] Events { get; set; }
    
    [JsonProperty("inheritance")]
    internal string[]? Inheritance { get; set; }
    
    [JsonProperty("operators")]
    internal Operator[]? Operators { get; set; }
}