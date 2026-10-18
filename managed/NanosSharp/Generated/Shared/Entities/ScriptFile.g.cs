#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Interop.Reflection;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A File represents an entry to a system file.</summary>
[ScriptClass("File")]
public partial class ScriptFile : Entity
{
    private static readonly MethodSlot CloseSlot = new("Close");
    private static readonly MethodSlot FlushSlot = new("Flush");
    private static readonly MethodSlot HasFailedSlot = new("HasFailed");
    private static readonly MethodSlot IsBadSlot = new("IsBad");
    private static readonly MethodSlot IsEOFSlot = new("IsEOF");
    private static readonly MethodSlot IsGoodSlot = new("IsGood");
    private static readonly MethodSlot ReadSlot = new("Read");
    private static readonly MethodSlot ReadAsyncSlot = new("ReadAsync");
    private static readonly MethodSlot ReadJSONSlot = new("ReadJSON");
    private static readonly MethodSlot ReadJSONAsyncSlot = new("ReadJSONAsync");
    private static readonly MethodSlot ReadLineSlot = new("ReadLine");
    private static readonly MethodSlot SeekSlot = new("Seek");
    private static readonly MethodSlot SizeSlot = new("Size");
    private static readonly MethodSlot SkipSlot = new("Skip");
    private static readonly MethodSlot TellSlot = new("Tell");
    private static readonly MethodSlot WriteSlot = new("Write");

    /// <summary>Checks if the file status is Bad</summary>
    public bool Bad => IsBad();

    /// <summary>Checks if the file status is End of File</summary>
    public bool EOF => IsEOF();

    /// <summary>Checks if the file status is Good</summary>
    public bool Good => IsGood();

    /// <summary>Spawns a new ScriptFile.</summary>
    public ScriptFile(string filePath, bool? truncate = null)
    {
        SpawnSelf("File", ScriptArguments.Trim([filePath, truncate], 1));
    }

    protected ScriptFile()
    {
    }

    /// <summary>Closes the file and destroys the entity</summary>
    public void Close() => Call(CloseSlot, 0);

    /// <summary>Flushes content to the file</summary>
    public void Flush() => Call(FlushSlot, 0);

    /// <summary>Checks if the last operation has Failed</summary>
    public bool HasFailed() => CallResult<bool>(HasFailedSlot, 0)!;

    /// <summary>Checks if the file status is Bad</summary>
    public bool IsBad() => CallResult<bool>(IsBadSlot, 0)!;

    /// <summary>Checks if the file status is End of File</summary>
    public bool IsEOF() => CallResult<bool>(IsEOFSlot, 0)!;

    /// <summary>Checks if the file status is Good</summary>
    public bool IsGood() => CallResult<bool>(IsGoodSlot, 0)!;

    /// <summary>Reads characters from the File and returns it. Also moves the file pointer to the latest read position. Pass 0 to read the whole file</summary>
    public string Read(int? length = null) => CallResult<string, int?>(ReadSlot, 0, length)!;

    /// <summary>Reads characters from the File asynchronously.</summary>
    public void ReadAsync(int? length, Delegate callback) =>
        Call(ReadAsyncSlot, 2, length, ScriptCallback.From(callback));

    /// <summary>Reads the whole file as a JSON and returns it.</summary>
    public object? ReadJSON() => CallResult<object?>(ReadJSONSlot, 0);

    /// <summary>Reads the whole file as a JSON and returns it asynchronously.</summary>
    public void ReadJSONAsync(Delegate callback) => Call(ReadJSONAsyncSlot, 1, ScriptCallback.From(callback));

    /// <summary>Reads and returns the next file line</summary>
    public string ReadLine() => CallResult<string>(ReadLineSlot, 0)!;

    /// <summary>Sets the file pointer to a specific position</summary>
    public void Seek(int position) => Call(SeekSlot, 1, position);

    /// <summary>Returns the size of the file</summary>
    public int Size() => CallResult<int>(SizeSlot, 0)!;

    /// <summary>Skips n (amount) positions from the current file pointer position</summary>
    public void Skip(int amount) => Call(SkipSlot, 1, amount);

    /// <summary>Returns the current file pointer position</summary>
    public int Tell() => CallResult<int>(TellSlot, 0)!;

    /// <summary>Writes the Data at the current position of the file</summary>
    public void Write(string data) => Call(WriteSlot, 1, data);
}
