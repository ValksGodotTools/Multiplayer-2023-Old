namespace Multiplayer;

public class GameServer : ENetServer
{
    private ConcurrentQueue<Cmd<Type>> Cmds { get; } = new();
    public Dictionary<uint, ServerPlayerData> Players { get; set; } = new();

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

    public void Enqueue(Type type, params object[] data) =>
        Cmds.Enqueue(new Cmd<Type>(type, data));

    protected override void ConcurrentQueues()
    {
        base.ConcurrentQueues();

        while (Cmds.TryDequeue(out Cmd<Type> cmd))
        {
            if (cmd.Opcode == (typeof(SPacketPlayerPositions)))
            {
                var playerPositions = (Dictionary<uint, Vector2>)cmd.Data[0];

                foreach (var player in Players)
                {
                    player.Value.Position = playerPositions[player.Key];
                }
            }
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
