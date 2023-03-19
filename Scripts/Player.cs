namespace Sandbox2;

using GodotUtils.TopDown;

public partial class Player : PlayerController
{
    private GTimer NetTimer { get; set; }
    private Vector2 PrevPos { get; set; }

    public override void _Ready()
    {
        NetTimer = new GTimer(this, NetTimerUpdate, 100)
        {
            Loop = true
        };

        NetTimer.Start();
    }

    private void NetTimerUpdate()
    {
        // no need to send to other players if there are no other players to send to
        if (GameMaster.OtherPlayers.Count == 0)
            return;

        // only send position if previous position is greater than 5 pixels
        if (PrevPos.DistanceTo(Position) > 5)
            Net.Client.Send(new CPacketPlayerPosition
            {
                Position = Position
            });

        PrevPos = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!UIConsole.IsVisible)
            base._PhysicsProcess(delta);
    }
}
