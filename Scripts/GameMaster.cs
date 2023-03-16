namespace Sandbox2;

public partial class GameMaster : Node
{
    private static Dictionary<uint, PlayerData> OtherPlayers { get; set; } = new();
    private static GameMaster Instance { get; set; }

    public static void UpdateOtherPlayers(Dictionary<uint, PlayerData> otherPlayers)
    {
        OtherPlayers = otherPlayers;

        foreach (var player in otherPlayers)
        {
            player.Value.Node2D.Position = player.Value.Position;
        }
    }

    public static void CreateOtherPlayer(uint id, PlayerData playerData)
    {
        Logger.Log($"[GameMaster] Creating other player {id}");
        var sprite = new Sprite2D
        {
            Texture = GD.Load<Texture2D>("res://icon.svg"),
            Position = playerData.Position
        };
        playerData.Node2D = sprite;

        Instance.AddChild(sprite);
        OtherPlayers.Add(id, playerData);
    }

    public override void _Ready()
    {
        Instance = this;
    }
}
