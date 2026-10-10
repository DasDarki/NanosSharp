#nullable enable

using NanosSharp.Entities;
using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Core;

/// <summary>TOML library.</summary>
public static partial class TOML
{
    private static readonly StaticMethodSlot DumpSlot = new("TOML", "Dump");
    private static readonly StaticMethodSlot ParseSlot = new("TOML", "Parse");

    /// <summary>Returns a string representing value encoded in TOML</summary>
    public static string Dump(object? value) => ScriptApi.CallResult<string, object?>(DumpSlot, 1, value)!;

    /// <summary>Returns a value representing the decoded TOML string</summary>
    public static object? Parse(string value) => ScriptApi.CallResult<object?, string>(ParseSlot, 1, value);
}
