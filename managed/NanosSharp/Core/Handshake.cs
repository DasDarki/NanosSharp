using System.Reflection;

namespace NanosSharp.Core;

internal static class Handshake
{
    public const string EventName = "nanossharp:handshake";
    public const string RequestEventName = "nanossharp:handshake-request";
    public const string ClientValueKey = "NanosSharp";
    public const int ProtocolVersion = 1;
    public const int MissingProtocolVersion = 0;
    public const string MarkerFileName = "nanossharp.json";
    public const string PackageValuePrefix = "NanosSharp:";

    public static string RuntimeVersion { get; } =
        typeof(Handshake).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "unknown";
}
