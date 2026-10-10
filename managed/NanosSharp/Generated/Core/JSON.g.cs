#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>JSON library.</summary>
public static partial class JSON
{
    private static readonly StaticMethodSlot ParseSlot = new("JSON", "parse");
    private static readonly StaticMethodSlot StringifySlot = new("JSON", "stringify");

    /// <summary>Returns a value representing the decoded JSON string</summary>
    public static object? Parse(string value) => ScriptApi.CallResult<object?, string>(ParseSlot, 1, value);

    /// <summary>Returns a string representing value encoded in JSON</summary>
    public static string Stringify(object? value) => ScriptApi.CallResult<string, object?>(StringifySlot, 1, value)!;
}
