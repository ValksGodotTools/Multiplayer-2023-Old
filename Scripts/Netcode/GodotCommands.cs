namespace Sandbox2;

public static class GodotCommands
{
    private static ConcurrentQueue<Cmd<GodotCmdOpcode>> GodotCmds { get; set; } = new();

    public static void Enqueue(GodotCmdOpcode opcode, params object[] data) =>
        GodotCmds.Enqueue(new Cmd<GodotCmdOpcode>(opcode, data));

    public static void Update()
    {
        if (GodotCmds.TryDequeue(out Cmd<GodotCmdOpcode> cmd))
        {

        }
    }
}

public enum GodotCmdOpcode
{
    
}
