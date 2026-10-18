namespace NanosSharp.Entities;

public partial class Database
{
    /// <summary>Executes a query without blocking the server and returns the amount of affected rows.</summary>
    public Task<long> ExecuteAsync(string query, params object?[] parameters)
    {
        var completion = new TaskCompletionSource<long>();
        ExecuteAsync(query, (long affectedRows, string? error) => Complete(completion, affectedRows, error), parameters);
        return completion.Task;
    }

    /// <summary>Runs a SELECT query without blocking the server and returns the fetched rows.</summary>
    public Task<IReadOnlyList<IReadOnlyDictionary<string, object?>>> SelectAsync(string query, params object?[] parameters)
    {
        var completion = new TaskCompletionSource<IReadOnlyList<IReadOnlyDictionary<string, object?>>>();
        SelectAsync(query, (Dictionary<string, object?>[]? rows, string? error) => Complete(completion, rows ?? [], error),
            parameters);
        return completion.Task;
    }

    private static void Complete<T>(TaskCompletionSource<T> completion, T result, string? error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            completion.TrySetException(new DatabaseException(error));
            return;
        }

        completion.TrySetResult(result);
    }
}
