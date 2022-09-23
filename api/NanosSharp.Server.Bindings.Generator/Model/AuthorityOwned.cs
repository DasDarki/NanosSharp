using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model
{
    internal class AuthorityOwned : Document
    {
        private const string AuthorityClient = "client";
    
        [JsonProperty("authority")]
        internal string Authority { get; set; }
        
        internal bool IsAllowed => Authority != AuthorityClient;
    }
}