namespace NanosSharp.Core;

/// <summary>Response sent by a route of the built in HTTP server.</summary>
public sealed record HttpServerResponse(
    int Status,
    string Data = "",
    string ContentType = "text/plain",
    IReadOnlyDictionary<string, string>? Headers = null)
{
    /// <summary>Creates a response with a JSON body.</summary>
    public static HttpServerResponse Json(string json, int status = 200) => new(status, json, "application/json");
}
