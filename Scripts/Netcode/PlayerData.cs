namespace Multiplayer;

public class ClientPlayerData
{
    public string Username { get; set; }
    public PrevCurQueue<Vector2> PrevCurPosition { get; set; } = new(Heartbeat.PositionUpdate);
    public Node2D Node2D { get; set; }
}

public class ServerPlayerData
{
    public string Username { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
}

public enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down
}
