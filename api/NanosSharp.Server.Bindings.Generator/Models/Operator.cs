using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class Operator
{
    [JsonProperty("name")]
    internal string Name { get; set; }
    
    [JsonProperty("operator")]
    internal string OperatorString { get; set; }
    
    [JsonProperty("lhs")]
    internal string Lhs { get; set; }
    
    [JsonProperty("rhs")]
    internal string Rhs { get; set; }
    
    [JsonProperty("return")]
    internal string Return { get; set; }
    
    [JsonProperty("description")]
    internal string? Description { get; set; }
}