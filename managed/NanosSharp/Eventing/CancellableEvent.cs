namespace NanosSharp.Eventing;

/// <summary>An event that can be cancelled, which tells nanos world to block the action.</summary>
public abstract class CancellableEvent : ScriptEvent
{
    /// <summary>Whether a handler cancelled the event.</summary>
    public bool IsCancelled { get; set; }

    /// <summary>Cancels the event.</summary>
    public void Cancel() => IsCancelled = true;
}
