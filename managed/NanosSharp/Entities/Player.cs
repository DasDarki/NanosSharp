namespace NanosSharp.Entities;

public partial class Player
{
    /// <summary>All players currently connected.</summary>
    public static IEnumerable<Player> All => Binding.EntityRegistry.All.OfType<Player>().Where(player => player.IsValid);
}
