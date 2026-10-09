using System.Collections;
using NanosSharp.Tests.Framework;

namespace NanosSharp.Tests.Tests;

public static class ExportTests
{
    [Test]
    public static void LuaFunctionCanBeCalled()
    {
        Assert.Equal(5L, Exports.Call<long>("LuaInteropLibrary.Add", 2, 3));
        Assert.Equal("hello nanos", Exports.Call<string>("LuaInteropLibrary.Greet", "nanos"));
    }

    [Test]
    public static void LuaTableCanBeRead()
    {
        var library = Exports.Get("LuaInteropLibrary") as IDictionary;
        Assert.True(library is not null, "LuaInteropLibrary is not a table");
        Assert.True(library!["Add"] is ScriptFunction, "Add is not a function");
    }

    [Test]
    public static void ExportedClassIsCallableFromLua()
    {
        Assert.Equal(42L, Exports.Call<long>("LuaInteropUseCSharp", 6, 7));
    }

    [Test]
    public static void ExportedMethodIsCallableFromLua()
    {
        Assert.Equal("NANOS", Exports.Call<string>("LuaInteropUseCSharpFunction", "nanos"));
    }

    [Test]
    public static void CallbackIsPassedToLua()
    {
        var doubled = ScriptCallback.From((Func<long, long>)(value => value * 2));
        Assert.Equal(42L, Exports.Call<long>("LuaInteropLibrary.Invoke", doubled, 21));
    }

    [Test]
    public static void ValuesRoundTrip()
    {
        Exports.Set("NanosSharpTestValue", new Dictionary<string, object?> { ["count"] = 12, ["name"] = "test" });
        Assert.Equal(12L, Exports.Get<long>("NanosSharpTestValue.count"));
        Assert.Equal("test", Exports.Get<string>("NanosSharpTestValue.name"));
    }

    [Test]
    public static void ExportedDelegateIsCallable()
    {
        Exports.Set("NanosSharpTestDelegate", (Func<string, string>)(text => text + "!"));
        Assert.Equal("nanos!", Exports.Call<string>("NanosSharpTestDelegate", "nanos"));
    }

    [Test]
    public static void MissingGlobalIsNull()
    {
        Assert.True(Exports.Get("NanosSharpDoesNotExist") is null, "Missing global is not null");
        Assert.Throws<ScriptException>(() => Exports.Call("NanosSharpDoesNotExist"));
    }

    [Export("CSharpInteropLibrary")]
    public static class InteropLibrary
    {
        public static int Multiply(int first, int second) => first * second;
    }

    [Export("NanosSharpShout")]
    public static string Shout(string text) => text.ToUpperInvariant();
}
