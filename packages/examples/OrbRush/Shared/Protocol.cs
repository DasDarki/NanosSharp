namespace OrbRush;

/// <summary>Event names and tuning values shared between the server and client sides of Orb Rush.</summary>
public static class Protocol
{
    /// <summary>Wire protocol version, bumped whenever the event payloads change.</summary>
    public const int Version = 1;

    /// <summary>Broadcast to every client with the current leaderboard text.</summary>
    public const string ScoreUpdate = "orbrush:score-update";

    /// <summary>Broadcast when a fresh round begins, carries the round length in seconds.</summary>
    public const string RoundStarted = "orbrush:round-started";

    /// <summary>Broadcast when a round ends, carries the winner name and their score.</summary>
    public const string RoundEnded = "orbrush:round-ended";

    /// <summary>Sent to the collecting client, carries their new personal score.</summary>
    public const string OrbCollected = "orbrush:orb-collected";

    /// <summary>Sent from a client that wants to locate the nearest orb.</summary>
    public const string RequestPing = "orbrush:request-ping";

    /// <summary>Sent back to the requesting client, carries the offset and distance to the nearest orb.</summary>
    public const string PingResult = "orbrush:ping-result";

    /// <summary>How long a single round lasts, in seconds.</summary>
    public const float RoundSeconds = 90f;

    /// <summary>How many orbs are active in the arena at once.</summary>
    public const int OrbCount = 8;
}
