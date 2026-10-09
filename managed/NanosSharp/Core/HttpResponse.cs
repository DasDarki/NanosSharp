namespace NanosSharp.Core;

/// <summary>Response of an HTTP request made through nanos world.</summary>
public sealed record HttpResponse(int Status, string Data, IReadOnlyDictionary<string, string> Headers)
{
    /// <summary>Whether the status code is in the 2xx range.</summary>
    public bool IsSuccess => Status is >= 200 and < 300;
}
