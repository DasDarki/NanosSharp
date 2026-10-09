#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>HTTP Requests Interface.</summary>
public static partial class HTTP
{
    private static readonly StaticMethodSlot RequestSlot = new("HTTP", "Request");
    private static readonly StaticMethodSlot RequestAsyncSlot = new("HTTP", "RequestAsync");
    private static readonly StaticMethodSlot SetConnectionTimeoutSlot = new("HTTP", "SetConnectionTimeout");
    private static readonly StaticMethodSlot SetReadWriteTimeoutSlot = new("HTTP", "SetReadWriteTimeout");

    /// <summary>Makes a synchronous HTTP Request</summary>
    public static IReadOnlyDictionary<string, object?> Request(
        string uri,
        string? endpoint = null,
        object? method = null,
        string? data = null,
        string? contentType = null,
        bool? compress = null,
        IDictionary<string, string>? headers = null) =>
        ScriptApi.CallResult<IReadOnlyDictionary<string, object?>, string, string?, object?, string?, string?, bool?, IDictionary<string, string>?>(RequestSlot, 1, uri, endpoint, method, data, contentType, compress, headers)!;

    /// <summary>Makes an asynchronous HTTP Request</summary>
    public static void RequestAsync(
        string uri,
        string? endpoint,
        object? method,
        string? data,
        string? contentType,
        bool? compress,
        IDictionary<string, string>? headers,
        Delegate callback) =>
        ScriptApi.Call(RequestAsyncSlot, 8, uri, endpoint, method, data, contentType, compress, headers, ScriptCallback.From(callback));

    /// <summary>Sets the HTTP requests global Connection Timeout in seconds</summary>
    public static void SetConnectionTimeout(uint connectionTimeout) =>
        ScriptApi.Call(SetConnectionTimeoutSlot, 1, connectionTimeout);

    /// <summary>Sets the HTTP requests global Read and Write Timeout in seconds</summary>
    public static void SetReadWriteTimeout(uint readWriteTimeout) =>
        ScriptApi.Call(SetReadWriteTimeoutSlot, 1, readWriteTimeout);
}
