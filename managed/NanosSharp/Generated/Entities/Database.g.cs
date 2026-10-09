#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>The Database entity provides programmers a way to access SQL databases easily through scripting.</summary>
[ScriptClass("Database")]
public partial class Database : Entity
{
    private static readonly MethodSlot CloseSlot = new("Close");
    private static readonly MethodSlot ExecuteSlot = new("Execute");
    private static readonly MethodSlot ExecuteAsyncSlot = new("ExecuteAsync");
    private static readonly MethodSlot SelectSlot = new("Select");
    private static readonly MethodSlot SelectAsyncSlot = new("SelectAsync");

    /// <summary>Spawns a new Database.</summary>
    public Database(DatabaseEngine databaseEngine, string connectionString, int? poolSize = null)
    {
        SpawnSelf("Database", ScriptArguments.Trim([databaseEngine, connectionString, poolSize], 2));
    }

    protected Database()
    {
    }

    /// <summary>Closes the Database</summary>
    public void Close() => Call(CloseSlot, 0);

    /// <summary>Executes a query synchronously, blocking until it finishes. Prefer <c>ExecuteAsync</c> to avoid freezing the server</summary>
    public object?[] Execute(string query, params object?[] parameters) =>
        InvokeVariadic<object?[]>(ExecuteSlot, [query], parameters)!;

    /// <summary>Execute a query asynchronously</summary>
    public void ExecuteAsync(string query, Delegate callback, params object?[] parameters) =>
        InvokeVariadic(ExecuteAsyncSlot, [query, ScriptCallback.From(callback)], parameters);

    /// <summary>Runs a SELECT query synchronously, blocking until it finishes. Prefer <c>SelectAsync</c> to avoid freezing the server</summary>
    public object?[] Select(string query, params object?[] parameters) =>
        InvokeVariadic<object?[]>(SelectSlot, [query], parameters)!;

    /// <summary>Execute a select query asynchronously</summary>
    public void SelectAsync(string query, Delegate callback, params object?[] parameters) =>
        InvokeVariadic(SelectAsyncSlot, [query, ScriptCallback.From(callback)], parameters);
}
