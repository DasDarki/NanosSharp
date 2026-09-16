using System.Text.Json;
using System.Text.Json.Serialization;
using Tomlyn.Serialization;

namespace NanosSharp.Host.Configuration;

[TomlSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace)]
[TomlSerializable(typeof(HostSettings))]
internal sealed partial class HostSettingsContext : TomlSerializerContext;
