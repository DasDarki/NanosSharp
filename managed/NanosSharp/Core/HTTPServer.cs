using NanosSharp.Enums;
using NanosSharp.Logging;

namespace NanosSharp.Core;

public static partial class HTTPServer
{
    private const int InternalServerError = 500;

    /// <summary>
    /// Registers a route below /scripting that answers every request with the handler result, the server must run
    /// with --enable_http_server.
    /// </summary>
    public static HttpRoute Route(HTTPMethod method, string path, Func<HttpServerRequest, HttpServerResponse> handler) =>
        Route(method, path, request => Task.FromResult(handler(request)));

    /// <summary>Registers a route below /scripting whose asynchronous handler answers every request.</summary>
    public static HttpRoute Route(HTTPMethod method, string path, Func<HttpServerRequest, Task<HttpServerResponse>> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        RegisterRoute(method, path,
            (ulong requestId, string? body, Dictionary<string, string>? query, Dictionary<string, string>? headers,
                    string? remoteAddress, int remotePort) =>
                _ = HandleAsync(requestId, handler,
                    new HttpServerRequest(method, path, body ?? string.Empty, query ?? [], headers ?? [], remoteAddress ?? string.Empty,
                        remotePort)));
        return new HttpRoute(method, path);
    }

    private static async Task HandleAsync(ulong requestId, Func<HttpServerRequest, Task<HttpServerResponse>> handler,
        HttpServerRequest request)
    {
        HttpServerResponse response;
        try
        {
            response = await handler(request);
        }
        catch (Exception exception)
        {
            Log.Exception(exception, $"Unhandled exception in HTTP route {request.Method} {request.Path}");
            response = new HttpServerResponse(InternalServerError);
        }

        var headers = response.Headers is null ? null : new Dictionary<string, string>(response.Headers);
        Respond(requestId, response.Status, response.Data, response.ContentType, headers);
    }
}
