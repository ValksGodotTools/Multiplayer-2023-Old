namespace Sandbox2;

public partial class GameMaster : Node
{
    public static uint PeerId { get; set; }
    public static Dictionary<uint, PlayerData> Players { get; } = new();

    public static void AddPlayer(uint id) => Players.Add(id, new PlayerData());
    public static void UpdatePlayerPosition(uint id, Vector2 pos)
    {
        if (Players.ContainsKey(id))
            Players[id].Position = pos;
        else
            Net.Client.Log($"{id} does not exist in Players", ConsoleColor.Yellow);
    }

    public override void _PhysicsProcess(double delta)
    {
        Net.Client.HandlePackets();
    }
}
