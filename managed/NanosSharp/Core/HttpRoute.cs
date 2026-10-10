using NanosSharp.Enums;
using NanosSharp.Threading;

namespace NanosSharp.Core;

/// <summary>A registered route of the built in HTTP server, disposing it unregisters the route.</summary>
public sealed class HttpRoute : IDisposable
{
    /// <summary>The HTTP method of the route.</summary>
    public HTTPMethod Method { get; }

    /// <summary>The path of the route below /scripting.</summary>
    public string Path { get; }

    private bool _disposed;

    internal HttpRoute(HTTPMethod method, string path)
    {
        Method = method;
        Path = path;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        MainThread.Ensure();
        HTTPServer.UnregisterRoute(Method, Path);
    }
}
