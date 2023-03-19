namespace Sandbox2;

public partial class GameMaster : Node
{
    private static Dictionary<uint, PlayerData> Players { get; } = new();

    public static void AddPlayer(uint id) => Players.Add(id, new PlayerData());
    public static void UpdatePlayerPosition(uint id, Vector2 pos) =>
        Players[id].Position = pos;

    public override void _PhysicsProcess(double delta)
    {
        Net.Client.HandlePackets();
    }
}
