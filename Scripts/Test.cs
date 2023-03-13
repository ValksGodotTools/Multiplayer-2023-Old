namespace Sandbox2;

public partial class Test : Node
{
	public override void _Ready()
	{
		AddChild(new UIConsole());
	}
}
