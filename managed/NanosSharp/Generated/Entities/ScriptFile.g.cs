#nullable enable

using NanosSharp.Entities.Binding;
using NanosSharp.Enums;
using NanosSharp.Mathematics;
using NanosSharp.Scripting;

namespace NanosSharp.Entities;

/// <summary>A File represents an entry to a system file.</summary>
[ScriptClass("File")]
public partial class ScriptFile : Entity
{
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
    public void Close() => Invoke("Close");

    /// <summary>Flushes content to the file</summary>
    public void Flush() => Invoke("Flush");

    /// <summary>Checks if the last operation has Failed</summary>
    public bool HasFailed() => Invoke<bool>("HasFailed")!;

    /// <summary>Checks if the file status is Bad</summary>
    public bool IsBad() => Invoke<bool>("IsBad")!;

    /// <summary>Checks if the file status is End of File</summary>
    public bool IsEOF() => Invoke<bool>("IsEOF")!;

    /// <summary>Checks if the file status is Good</summary>
    public bool IsGood() => Invoke<bool>("IsGood")!;

    /// <summary>Reads characters from the File and returns it. Also moves the file pointer to the latest read position. Pass 0 to read the whole file</summary>
    public string Read(int? length = null) => Invoke<string>("Read", ScriptArguments.Trim([length], 0))!;

    /// <summary>Reads characters from the File asynchronously.</summary>
    public void ReadAsync(int? length, Delegate callback) => Invoke("ReadAsync", length, ScriptCallback.From(callback));

    /// <summary>Reads the whole file as a JSON and returns it.</summary>
    public object? ReadJSON() => Invoke<object?>("ReadJSON");

    /// <summary>Reads the whole file as a JSON and returns it asynchronously.</summary>
    public void ReadJSONAsync(Delegate callback) => Invoke("ReadJSONAsync", ScriptCallback.From(callback));

    /// <summary>Reads and returns the next file line</summary>
    public string ReadLine() => Invoke<string>("ReadLine")!;

    /// <summary>Sets the file pointer to a specific position</summary>
    public void Seek(int position) => Invoke("Seek", position);

    /// <summary>Returns the size of the file</summary>
    public int Size() => Invoke<int>("Size")!;

    /// <summary>Skips n (amount) positions from the current file pointer position</summary>
    public void Skip(int amount) => Invoke("Skip", amount);

    /// <summary>Returns the current file pointer position</summary>
    public int Tell() => Invoke<int>("Tell")!;

    /// <summary>Writes the Data at the current position of the file</summary>
    public void Write(string data) => Invoke("Write", data);
}
