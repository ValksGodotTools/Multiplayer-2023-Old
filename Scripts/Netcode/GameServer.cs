namespace Sandbox2;

public class GameServer : ENetServer
{
    public Dictionary<uint, PlayerData> Players { get; set; } = new();

    /// <summary>
    /// Get all players except for one player
    /// </summary>
    /// <param name="id">The player id to exclude</param>
    public Dictionary<uint, PlayerData> GetOtherPlayers(uint id) =>
        Players.Where(x => x.Key != id).ToDictionary(x => x.Key, x => x.Value);

    public GameServer()
    {
        UpdateTimer.SetDelay(5000);
    }

    protected override void Update()
    {
        foreach (var player in Players)
        {
            // Get all the player positions except for 'player'
            var otherPlayerPositions = GetOtherPlayers(player.Key)
                .ToDictionary(x => x.Key, x => x.Value.Position);

            // No other players in the server, don't send anything
            if (otherPlayerPositions.Count == 0)
                return;

            // Send the 'other player positions' to this player
            Send(new SPacketPlayerPositions
            {
                PlayerPositions = otherPlayerPositions
            }, Peers[player.Key]);
        }
    }

    protected override void Disconnected(Event netEvent)
    {
        Players.Remove(netEvent.Peer.ID);
    }

    protected override void Stopped()
    {
        Net.Server = new();
    }
}
