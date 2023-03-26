namespace Multiplayer;

using GodotUtils.TopDown;

public partial class Player : PlayerController
{
    private GTimer NetTimer { get; set; }

    private bool PressedUp    { get; set; }
    private bool PressedDown  { get; set; }
    private bool PressedLeft  { get; set; }
    private bool PressedRight { get; set; }

    public override void _Ready()
    {
        NetTimer = new GTimer(this, NetTimerUpdate, Heartbeat.PositionUpdate)
        {
            Loop = true
        };

        NetTimer.Start();
    }

    private void NetTimerUpdate()
    {
        var horz = Direction.None;
        var vert = Direction.None;

        if (PressedLeft) horz = Direction.Left;
        if (PressedRight) horz = Direction.Right;
        if (PressedDown) vert = Direction.Down;
        if (PressedUp) vert = Direction.Up;

        Net.Client.Send(new CPacketPlayerMove
        {
            Horizontal = horz,
            Vertical = vert
        });
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!UIConsole.IsVisible)
        {
            base._PhysicsProcess(delta);

            PressedUp    = Input.IsActionPressed("player_move_up");
            PressedDown  = Input.IsActionPressed("player_move_down");
            PressedLeft  = Input.IsActionPressed("player_move_left");
            PressedRight = Input.IsActionPressed("player_move_right");
        }
    }
}
