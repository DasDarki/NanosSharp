using NanosSharp.Enums;

namespace NanosSharp.Core;

public static partial class HTTP
{
    private const string DefaultContentType = "application/json";

    /// <summary>Makes an asynchronous HTTP request that completes on the main thread.</summary>
    public static Task<HttpResponse> RequestAsync(
        string uri,
        string endpoint = "",
        HTTPMethod method = HTTPMethod.GET,
        string data = "",
        string contentType = DefaultContentType,
        bool compress = false,
        IDictionary<string, string>? headers = null)
    {
        var completion = new TaskCompletionSource<HttpResponse>();
        RequestAsync(uri, endpoint, method, data, contentType, compress, headers ?? new Dictionary<string, string>(),
            (int status, string? response, Dictionary<string, string>? responseHeaders) =>
                completion.TrySetResult(new HttpResponse(status, response ?? string.Empty, responseHeaders ?? [])));
        return completion.Task;
    }
}
