namespace Sandbox2;

using GodotUtils.TopDown;

public partial class Player : PlayerController
{
    public override void _PhysicsProcess(double delta)
    {
        if (!UIConsole.IsVisible)
            base._PhysicsProcess(delta);
    }
}
