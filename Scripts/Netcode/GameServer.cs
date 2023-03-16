namespace Sandbox2;

public class GameServer : ENetServer
{
    public Dictionary<uint, PlayerData> Players { get; set; } = new();
    public STimer ServerLoop { get; }

    public GameServer()
    {
        ServerLoop = new STimer(100, () =>
        {
            // Loop through all the players
            foreach (var player in Players)
            {
                // Get all the player positions except for 'player'
                var otherPlayerPositions = GetAllPlayersExceptOne(player.Key)
                    .ToDictionary(x => x.Key, x => x.Value.Position);

                // No other players in the server, don't send anything
                if (otherPlayerPositions.Count == 0)
                    continue;

                // Send the 'other player positions' to this player
                Send(new SPacketPlayerPositions
                {
                    PlayerPositions = otherPlayerPositions
                }, Peers[player.Key]);
            }
        }, false);
    }

    private Dictionary<uint, PlayerData> GetAllPlayersExceptOne(uint id) =>
        Players.Where(x => x.Key != id).ToDictionary(x => x.Key, x => x.Value);

    protected override void Starting()
    {
        ServerLoop.Start();
    }

    protected override void Stopping()
    {
        ServerLoop.Stop();
        ServerLoop.Dispose();
    }

    protected override void Disconnected(Event netEvent)
    {
        Players.Remove(netEvent.Peer.ID);
    }
}
