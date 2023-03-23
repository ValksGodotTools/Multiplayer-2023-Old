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

    private double progress;

    public override void _PhysicsProcess(double delta)
    {
        Net.Client.HandlePackets();

        while (Net.Client.GodotCmds.TryDequeue(out Cmd<GodotOpcode> cmd))
        {
            var opcode = cmd.Opcode;

            if (opcode == GodotOpcode.Disconnected)
            {
                //var disconnectOpcode = (DisconnectOpcode)cmd.Data[0];

                // Remove main player
                GetNode("Player").QueueFree();

                // Remove other players
                var ids = OtherPlayers.Select(x => x.Key).ToList();

                for (int i = 0; i < ids.Count(); i++)
                    RemovePlayer(ids[i]);
            }
        }

        foreach (var player in OtherPlayers.Values)
        {
            player.PrevCurPosition.UpdateProgress(delta);

            var prevCur = player.PrevCurPosition;

            var prevPos = prevCur.Previous;
            var curPos = prevCur.Current;
            var t = (float)prevCur.Progress;

            // If 't' is > 1 then the player will continue moving in the last known direction
            if (t <= 1)
                player.Node2D.Position = prevPos.Lerp(curPos, t);
        }
    }

    public static void RemovePlayer(uint id)
    {
        OtherPlayers[id].Node2D.QueueFree();
        OtherPlayers.Remove(id);
    }

    public static void AddPlayer(uint id, Vector2 position)
    {
        var texture = GD.Load<Texture2D>("res://icon.svg");
        var sprite = new Sprite2D
        {
            Texture = texture,
            Position = position
        };

        Instance.AddChild(sprite);

        OtherPlayers.Add(id, new PlayerData
        {
            Node2D = sprite
        });
    }
    public static void UpdatePlayerPosition(uint id, Vector2 pos)
    {
        if (!OtherPlayers.ContainsKey(id))
        {
            Net.Client.Log($"{id} does not exist in Players", ConsoleColor.Yellow);
            return;
        }

        OtherPlayers[id].PrevCurPosition.Add(pos);
    }
}
