namespace Sandbox2;

public class GameClient : ENetClient
{
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

    protected override void Stopped()
    {
        // This ensures all local values like Players are reset
        Net.Client = new();
    }
}

public enum GameClientOpcode
{

}
