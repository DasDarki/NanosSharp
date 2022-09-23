using Newtonsoft.Json;

namespace NanosSharp.Server.Bindings.Generator.Model
{
    internal class Document
    {
        [JsonProperty("name")]
        internal string Name { get; set; }
    
        [JsonProperty("description")]
        internal string Description { get; set; }
    
        [JsonProperty("description_long")]
        internal string DescriptionLong { get; set; }
        
        internal string VararglessName
        {
            get
            {
                var varargless = Name.EndsWith("...") ? Name[..^3] : Name;
                if (varargless == "object")
                {
                    return "obj";
                }

                return varargless;
            }
        }
    }
}