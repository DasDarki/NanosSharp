#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>HTTP Requests Interface.</summary>
public static partial class HTTP
{
    /// <summary>Makes a synchronous HTTP Request</summary>
    public static IReadOnlyDictionary<string, object?> Request(
        string uri,
        string? endpoint = null,
        object? method = null,
        string? data = null,
        string? contentType = null,
        bool? compress = null,
        IDictionary<string, string>? headers = null) =>
        ScriptApi.Invoke<IReadOnlyDictionary<string, object?>>("HTTP", "Request", ScriptArguments.Trim([uri, endpoint, method, data, contentType, compress, headers], 1))!;

    /// <summary>Makes an asynchronous HTTP Request</summary>
    public static void RequestAsync(
        string uri,
        string? endpoint = null,
        object? method = null,
        string? data = null,
        string? contentType = null,
        bool? compress = null,
        IDictionary<string, string>? headers = null,
        Delegate? callback = null) =>
        ScriptApi.Invoke("HTTP", "RequestAsync", ScriptArguments.Trim([uri, endpoint, method, data, contentType, compress, headers, ScriptCallback.From(callback)], 1));

    /// <summary>Sets the HTTP requests global Connection Timeout in seconds</summary>
    public static void SetConnectionTimeout(uint connectionTimeout) =>
        ScriptApi.Invoke("HTTP", "SetConnectionTimeout", connectionTimeout);

    /// <summary>Sets the HTTP requests global Read and Write Timeout in seconds</summary>
    public static void SetReadWriteTimeout(uint readWriteTimeout) =>
        ScriptApi.Invoke("HTTP", "SetReadWriteTimeout", readWriteTimeout);
}
