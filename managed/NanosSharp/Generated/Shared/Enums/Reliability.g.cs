#nullable enable

namespace NanosSharp.Enums;

/// <summary>Delivery guarantee of a network message</summary>
public enum Reliability
{
    /// <summary>Send the message unreliably, the message may be lost (with no retransmission) or received in a different order than it was sent, but is cheaper and faster to use than reliable. Use it for sending events that are transient and non critical for gameplay</summary>
    Unreliable = 0,
    /// <summary>Send the message reliably, this is guaranteed to be sent and received in order, but is more expensive to use than unreliable. Use it when you want to make sure the event is received by the other side</summary>
    Reliable = 1,
}
