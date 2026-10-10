#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>Routes on the built in HTTP server, served below /scripting/ when the server runs with --enable_http_server.</summary>
public static partial class HTTPServer
{
    private static readonly StaticMethodSlot RegisterRouteSlot = new("HTTPServer", "RegisterRoute");
    private static readonly StaticMethodSlot RespondSlot = new("HTTPServer", "Respond");
    private static readonly StaticMethodSlot UnregisterRouteSlot = new("HTTPServer", "UnregisterRoute");

    /// <summary>Registers a route, every request has to be answered with Respond</summary>
    public static void RegisterRoute(HTTPMethod method, string path, Delegate callback) =>
        ScriptApi.Call(RegisterRouteSlot, 3, method, path, ScriptCallback.From(callback));

    /// <summary>Answers a request received by a route</summary>
    public static void Respond(
        ulong requestId,
        int status,
        string? data = null,
        string? contentType = null,
        IDictionary<string, string>? headers = null) =>
        ScriptApi.Call(RespondSlot, 2, requestId, status, data, contentType, headers);

    /// <summary>Removes a route</summary>
    public static void UnregisterRoute(HTTPMethod method, string path) =>
        ScriptApi.Call(UnregisterRouteSlot, 2, method, path);
}
