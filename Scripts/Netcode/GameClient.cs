namespace Sandbox2;

public class GameClient : ENetClient
{
    protected override void Stopped()
    {
        // This ensures all local values like Players are reset
        Net.Client = new();
    }
}
