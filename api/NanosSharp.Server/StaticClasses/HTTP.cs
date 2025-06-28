using System.Net;
using NanosSharp.Server.Bindings;

namespace NanosSharp.Server;

/// <summary>
/// The class is the equivalent to the static Events class in C#.
/// </summary>
/// <remarks>
/// It is not recommended to use this class because C# as its own really good HTTP request implementation. The class
/// is just implemented for completeness and to allow compatibility with the Lua API.
/// </remarks>
public static class HTTP
{
    /// <summary>
    /// Makes an asynchronous HTTP request to the given URI with the specified parameters.
    /// </summary>
    /// <param name="uri">The URI to request.</param>
    /// <param name="endpoint">The endpoint to request, if any.</param>
    /// <param name="method">The HTTP method to use for the request. Defaults to GET.</param>
    /// <param name="data">The data to send with the request, if any.</param>
    /// <param name="content_type">The content type of the data being sent, if any.</param>
    /// <param name="compress">Whether to compress the request data. Defaults to false.</param>
    /// <param name="headers">Additional headers to include in the request.</param>
    /// <returns>A <see cref="Task{HttpResponse}"/> that represents the asynchronous operation, containing the response status and data.</returns>
    public static Task<HttpResponse> RequestAsync(string uri, string? endpoint = null, HTTPMethod method = HTTPMethod.GET, string? data = null, string? content_type = null, bool compress = false, Dictionary<string, object>? headers = null)
    {
        ServerModule.EnsureMainThread();
        
        var tcs = new TaskCompletionSource<HttpResponse>();
        
        BHTTP.RequestAsync(ServerModule.VM, uri, endpoint, method, data, content_type, compress, headers, (vm) =>
        {
            var res = (BHTTP.Request_Return0)vm.ToTable(-1);
            vm.Pop();
            tcs.SetResult(new HttpResponse((HttpStatusCode) res.Status, res.Data));
            return 0;
        });
        
        return tcs.Task;
    }
    
    /// <summary>
    /// Makes a synchronous HTTP request to the given URI with the specified parameters.
    /// </summary>
    /// <param name="uri">The URI to request.</param>
    /// <param name="endpoint">The endpoint to request, if any.</param>
    /// <param name="method">The HTTP method to use for the request. Defaults to GET.</param>
    /// <param name="data">The data to send with the request, if any.</param>
    /// <param name="content_type">The content type of the data being sent, if any.</param>
    /// <param name="compress">Whether to compress the request data. Defaults to false.</param>
    /// <param name="headers">Additional headers to include in the request.</param>
    /// <returns>A <see cref="HttpResponse"/> object containing the response status and data.</returns>
    public static HttpResponse Request(string uri, string? endpoint = null, HTTPMethod method = HTTPMethod.GET, string? data = null, string? content_type = null, bool compress = false, Dictionary<string, object>? headers = null)
    {
        ServerModule.EnsureMainThread();
        
        var res = BHTTP.Request(ServerModule.VM, uri, endpoint, method, data, content_type, compress, headers);
        
        return new HttpResponse((HttpStatusCode) res.Status, res.Data);
    }

    /// <summary>
    /// Sets the connection timeout for HTTP requests.
    /// </summary>
    /// <param name="timeout">The timeout in seconds.</param>
    public static void SetConnectionTimeout(long timeout)
    {
        ServerModule.EnsureMainThread();
        
        BHTTP.SetConnectionTimeout(ServerModule.VM, timeout);
    }
    
    /// <summary>
    /// Sets the read/write timeout for HTTP requests.
    /// </summary>
    /// <param name="timeout">The timeout in seconds.</param>
    public static void SetReadWriteTimeout(long timeout)
    {
        ServerModule.EnsureMainThread();
        
        BHTTP.SetReadWriteTimeout(ServerModule.VM, timeout);
    }
}