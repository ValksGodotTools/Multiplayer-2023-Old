namespace Multiplayer;

public class GameServer : ENetServer
{
    public Dictionary<uint, ServerPlayerData> Players { get; set; } = new();
    private float Delta { get; } = 1f / 60;

    /// <summary>
    /// Get all players except for one player
    /// </summary>
    /// <param name="id">The player id to exclude</param>
    public Dictionary<uint, ServerPlayerData> GetOtherPlayers(uint id) =>
        Players.Where(x => x.Key != id).ToDictionary(x => x.Key, x => x.Value);

    public GameServer()
    {
        EmitLoop.SetDelay(Heartbeat.PositionUpdate);
    }

    protected override void Simulation()
    {
        foreach (var player in Players.Values)
        {
            var speed = 50;
            player.Position += (player.Direction.Normalized() * speed * Delta).Lerp(Vector2.Zero, 0.1f);
        }
    }

    protected override void Emit()
    {
        if (Players.Count < 2)
            return;

        foreach (var player in Players)
        {
            // Get all the player positions except for 'player'
            // Send position to player only if within a distance
            var otherPlayerPositions = GetOtherPlayers(player.Key)
                .ToDictionary(x => x.Key, x => x.Value.Position);

            //GD.Print(otherPlayerPositions.PrintFull());

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

        // Tell everyone about the player that just left the server
        foreach (var player in Players)
        {
            Send(new SPacketPlayerJoinLeaves
            {
                Data = new()
                {
                    { 
                        netEvent.Peer.ID, 
                        new JoinLeaveData 
                        {
                            Joining = false
                        }
                    }
                }
            }, Peers[player.Key]);
        }
    }

    protected override void Stopped()
    {
        // This ensures all local values like Players are reset
        Net.Server = new();
    }
}
