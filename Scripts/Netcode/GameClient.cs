namespace Sandbox2;

public class GameClient : ENetClient
{
    protected override void Disconnect(Event netEvent)
    {
        base.Disconnect(netEvent);

        //var opcode = (DisconnectOpcode)netEvent.Data;

        
    }

    protected override void Stopped()
    {
        // This ensures all local values like Players are reset
        //Net.Client = new();
    }
}
