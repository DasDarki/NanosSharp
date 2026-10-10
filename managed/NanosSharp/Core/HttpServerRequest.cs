using NanosSharp.Enums;

namespace NanosSharp.Core;

/// <summary>Request received by a route of the built in HTTP server.</summary>
public sealed record HttpServerRequest(
    HTTPMethod Method,
    string Path,
    string Body,
    IReadOnlyDictionary<string, string> Query,
    IReadOnlyDictionary<string, string> Headers,
    string RemoteAddress,
    int RemotePort);
