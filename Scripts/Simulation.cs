namespace Multiplayer;

/*
 * There is no sane way to re-create the Godot physics engine in pure .NET C#
 * all over again. This is why the server pumps all the data it receives from
 * the clients to the simulation (which is running on the Godot thread). Then 
 * the simulation pumps this information back to the server so the server can 
 * send this information to everyone.
 */
public partial class Simulation : Node
{
    public static ConcurrentQueue<Cmd<Type>> Cmds { get; } = new();
    public static Dictionary<uint, SimulatedPlayer> Players { get; set; } = new();

    public static void Start()
    {
        Instance.SetPhysicsProcess(true);
        Instance.TimerEmit = new GTimer(Instance, Emit, Heartbeat.PositionUpdate);
        Instance.TimerEmit.Loop = true;
        Instance.TimerEmit.Start();
    }

    public static void Stop()
    {
        Instance.SetPhysicsProcess(false);
        Instance.TimerEmit?.Stop();
    }

    public static void Enqueue(Type type, params object[] data) =>
        Cmds.Enqueue(new Cmd<Type>(type, data));

    private static Simulation Instance { get; set; }

    private int SimulationLayer { get; } = 10;
    private GTimer TimerEmit { get; set; }

    public override void _Ready()
    {
        Instance = this;
        Stop();
    }

    public override void _PhysicsProcess(double delta)
    {
        while (Cmds.TryDequeue(out Cmd<Type> cmd))
        {
            if (cmd.Opcode == typeof(CPacketPlayerJoin))
            {
                var id = (uint)cmd.Data[0];
                var pos = (Vector2)cmd.Data[1];

                CreatePlayer(id, pos);
            }
            else if (cmd.Opcode == typeof(CPacketPlayerMove))
            {
                var id = (uint)cmd.Data[0];
                var dir = (Vector2)cmd.Data[1];

                Players[id].Direction = dir;
            }
        }

        foreach (var player in Players.Values)
        {
            var node = player.CharacterBody2D;
            var dir = player.Direction;
            var speed = 50;
            var friction = 0.1f;

            node.Velocity += dir * speed;
            node.Velocity = node.Velocity.Lerp(Vector2.Zero, friction);

            node.MoveAndSlide();
        }
    }

    private static void Emit()
    {
        var playerPositions = Players.ToDictionary(
            x => x.Key, 
            x => x.Value.CharacterBody2D.Position);

        Net.Server.Enqueue(typeof(SPacketPlayerPositions), playerPositions);
    }

    private void CreatePlayer(uint id, Vector2 position)
    {
        var player = GD.Load<PackedScene>("res://player.tscn").Instantiate<CharacterBody2D>();
        player.Position = position;
        player.Modulate = Colors.Green;
        player.SetCollisionMaskLayer(SimulationLayer);

        Players.Add(id, new SimulatedPlayer
        {
            CharacterBody2D = player
        });
        AddChild(player);
    }
}

public class SimulatedPlayer
{
    public CharacterBody2D CharacterBody2D { get; set; }
    public Vector2 Direction { get; set; }
}
