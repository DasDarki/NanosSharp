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
    public void Close() => Invoke(CloseSlot);

    /// <summary>Flushes content to the file</summary>
    public void Flush() => Invoke(FlushSlot);

    /// <summary>Checks if the last operation has Failed</summary>
    public bool HasFailed() => Invoke<bool>(HasFailedSlot)!;

    /// <summary>Checks if the file status is Bad</summary>
    public bool IsBad() => Invoke<bool>(IsBadSlot)!;

    /// <summary>Checks if the file status is End of File</summary>
    public bool IsEOF() => Invoke<bool>(IsEOFSlot)!;

    /// <summary>Checks if the file status is Good</summary>
    public bool IsGood() => Invoke<bool>(IsGoodSlot)!;

    /// <summary>Reads characters from the File and returns it. Also moves the file pointer to the latest read position. Pass 0 to read the whole file</summary>
    public string Read(int? length = null) => Invoke<string>(ReadSlot, ScriptArguments.Trim([length], 0))!;

    /// <summary>Reads characters from the File asynchronously.</summary>
    public void ReadAsync(int? length, Delegate callback) =>
        Invoke(ReadAsyncSlot, length, ScriptCallback.From(callback));

    /// <summary>Reads the whole file as a JSON and returns it.</summary>
    public object? ReadJSON() => Invoke<object?>(ReadJSONSlot);

    /// <summary>Reads the whole file as a JSON and returns it asynchronously.</summary>
    public void ReadJSONAsync(Delegate callback) => Invoke(ReadJSONAsyncSlot, ScriptCallback.From(callback));

    /// <summary>Reads and returns the next file line</summary>
    public string ReadLine() => Invoke<string>(ReadLineSlot)!;

    /// <summary>Sets the file pointer to a specific position</summary>
    public void Seek(int position) => Invoke(SeekSlot, position);

    /// <summary>Returns the size of the file</summary>
    public int Size() => Invoke<int>(SizeSlot)!;

    /// <summary>Skips n (amount) positions from the current file pointer position</summary>
    public void Skip(int amount) => Invoke(SkipSlot, amount);

    /// <summary>Returns the current file pointer position</summary>
    public int Tell() => Invoke<int>(TellSlot)!;

    /// <summary>Writes the Data at the current position of the file</summary>
    public void Write(string data) => Invoke(WriteSlot, data);
}
