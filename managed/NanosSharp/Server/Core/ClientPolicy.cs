namespace NanosSharp.Core;

internal sealed record ClientPolicy(bool IsRequired, TimeSpan HandshakeTimeout, string KickMessage);
