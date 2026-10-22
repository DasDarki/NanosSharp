namespace OrbRush;

/// <summary>Tracks every player's collected orb count for the running match. Injected into the game mode.</summary>
public sealed class ScoreBoard : Service
{
    private readonly Dictionary<uint, (Player Player, int Score)> _entries = new();

    /// <summary>Adds a player with a zero score when they join.</summary>
    public void Register(Player player)
    {
        _entries[player.ID] = (player, 0);
    }

    /// <summary>Awards one point to a player and returns their new score.</summary>
    public int Award(Player player)
    {
        var score = _entries.TryGetValue(player.ID, out var entry) ? entry.Score + 1 : 1;
        _entries[player.ID] = (player, score);
        return score;
    }

    /// <summary>Resets every known player back to zero for a new round.</summary>
    public void Reset()
    {
        foreach (var id in _entries.Keys.ToList())
        {
            _entries[id] = (_entries[id].Player, 0);
        }
    }

    /// <summary>Builds a compact, score sorted leaderboard line for the HUD.</summary>
    public string Leaderboard()
    {
        var lines = _entries.Values
            .Where(entry => entry.Player.IsValid)
            .OrderByDescending(entry => entry.Score)
            .Take(5)
            .Select(entry => $"{entry.Player.Name} {entry.Score}");
        var text = string.Join("   ", lines);
        return text.Length == 0 ? "no players yet" : text;
    }

    /// <summary>Returns the current leader, or null when nobody has scored yet.</summary>
    public (string Name, int Score)? Leader()
    {
        var top = _entries.Values
            .Where(entry => entry.Player.IsValid)
            .OrderByDescending(entry => entry.Score)
            .FirstOrDefault();
        if (top.Player is null || !top.Player.IsValid)
        {
            return null;
        }

        return (top.Player.Name, top.Score);
    }
}
