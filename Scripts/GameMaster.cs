namespace Sandbox2;

public partial class GameMaster : Node
{
    public static uint PeerId { get; set; }
    public static Dictionary<uint, PlayerData> OtherPlayers { get; } = new();
    private static GameMaster Instance { get; set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public static void AddPlayer(uint id)
    {
        var texture = GD.Load<Texture2D>("res://icon.svg");
        var sprite = new Sprite2D
        {
            Texture = texture,
            GlobalPosition = new Vector2(500, 500)
        };

        Instance.AddChild(sprite);

        OtherPlayers.Add(id, new PlayerData
        {
            Node2D = sprite
        });
    }
    public static void UpdatePlayerPosition(uint id, Vector2 pos)
    {
        if (OtherPlayers.ContainsKey(id))
            OtherPlayers[id].Position = pos;
        else
        {
            Net.Client.Log($"{id} does not exist in Players", ConsoleColor.Yellow);
            return;
        }

        Net.Client.Log(pos);

        OtherPlayers[id].Node2D.GlobalPosition = pos;
    }

    public override void _PhysicsProcess(double delta)
    {
        Net.Client.HandlePackets();
    }
}
