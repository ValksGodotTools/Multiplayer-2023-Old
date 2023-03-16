namespace Sandbox2;

public class GameClient : ENetClient
{
    private Dictionary<uint, Vector2> PlayerPositions { get; set; } = new();
    private ConcurrentQueue<Cmd<GameClientOpcode>> GameClientCmds { get; set; } = new();

    public void EnqueueCmd(GameClientOpcode opcode, params object[] data) =>
        GameClientCmds.Enqueue(new Cmd<GameClientOpcode>(opcode, data));

    protected override void ConcurrentQueues()
    {
        base.ConcurrentQueues();

        while (GameClientCmds.TryDequeue(out Cmd<GameClientOpcode> cmd))
        {
            
        }
    }
}

public enum GameClientOpcode
{
    
}
