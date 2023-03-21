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
        UpdateTimer.SetDelay(Heartbeat.PositionUpdate);
    }

    protected override void Update()
    {
        if (Players.Count < 2)
            return;

        foreach (var player in Players)
        {
            var prevPos = player.Value.PrevCurPosition.Previous;
            var curPos  = player.Value.PrevCurPosition.Current;

            if (prevPos.DistanceTo(curPos) < 10)
                continue;

            // Get all the player positions except for 'player'
            // Send position to player only if within a distance
            var otherPlayerPositions = GetOtherPlayers(player.Key)
                .Where(x => x.Value.PrevCurPosition.Previous.DistanceTo(prevPos) < 2000)
                .ToDictionary(x => x.Key, x => x.Value.PrevCurPosition.Previous);

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
        // This ensures all local values like Players are reset
        Net.Server = new();
    }
}
