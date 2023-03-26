namespace Multiplayer;

public class ClientPlayerData
{
    public string Username { get; set; }
    public PrevCurQueue<Vector2> PrevCurPosition { get; set; } = new(Heartbeat.PositionUpdate);
    public Node2D Node2D { get; set; }
}
