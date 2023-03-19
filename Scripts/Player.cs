namespace Sandbox2;

using GodotUtils.TopDown;

public partial class Player : PlayerController
{
    private GTimer NetTimer { get; set; }

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
        Net.Client.Send(new CPacketPlayerPosition
        {
            Position = Position
        });
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!UIConsole.IsVisible)
            base._PhysicsProcess(delta);
    }
}
