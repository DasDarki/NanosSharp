namespace NanosSharp.Entities;

public partial class ScriptFile
{
    /// <summary>Reads the file without blocking the server, a length of 0 reads the whole file.</summary>
    public Task<string> ReadAsync(int length = 0)
    {
        var completion = new TaskCompletionSource<string>();
        ReadAsync(length, (string? content) => completion.TrySetResult(content ?? string.Empty));
        return completion.Task;
    }

    /// <summary>Reads and parses the JSON file without blocking the server.</summary>
    public Task<object?> ReadJSONAsync()
    {
        var completion = new TaskCompletionSource<object?>();
        ReadJSONAsync((object? content) => completion.TrySetResult(content));
        return completion.Task;
    }
}
