using System.Reflection;

namespace NanosSharp.Tests.Framework;

public sealed class TestRunner : Package
{
    private const string Prefix = "[TEST]";

    private int _passed;
    private int _failed;

    protected override void OnStart() => Server.OnStart += RunAll;

    private async void RunAll()
    {
        var tests = typeof(TestRunner).Assembly.GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(method => method.GetCustomAttribute<TestAttribute>() is not null)
            .OrderBy(method => method.DeclaringType!.Name, StringComparer.Ordinal)
            .ThenBy(method => method.Name, StringComparer.Ordinal)
            .ToArray();

        foreach (var test in tests)
        {
            await RunAsync(test);
        }

        Log.Info($"{Prefix} SUMMARY passed={_passed} failed={_failed}");
        MainThread.Post(Server.Stop);
    }

    private async Task RunAsync(MethodInfo test)
    {
        var name = $"{test.DeclaringType!.Name}.{test.Name}";
        try
        {
            if (test.Invoke(null, null) is Task task)
            {
                await task;
            }

            _passed++;
            Log.Info($"{Prefix} PASS {name}");
        }
        catch (Exception exception)
        {
            _failed++;
            var cause = exception is TargetInvocationException { InnerException: { } inner } ? inner : exception;
            var location = cause.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "unknown location";
            Log.Error($"{Prefix} FAIL {name}: {cause.GetType().Name}: {cause.Message} ({location})");
        }
    }
}
