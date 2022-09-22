using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model;

internal class AuthorityOwned : Document
{
    internal const string AuthorityServer = "server";
    internal const string AuthorityClient = "client";
    internal const string AuthorityBoth = "both";
    
    [JsonProperty("authority")]
    internal string? Authority { get; set; }
}