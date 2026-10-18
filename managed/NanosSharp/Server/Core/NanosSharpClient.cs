namespace NanosSharp.Core;

/// <summary>The NanosSharp client runtime a player reported in the handshake.</summary>
/// <param name="Version">The NanosSharp version of the client.</param>
/// <param name="ProtocolVersion">The handshake protocol version of the client.</param>
public sealed record NanosSharpClient(string Version, int ProtocolVersion);
