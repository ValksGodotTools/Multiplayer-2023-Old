global using Godot;
global using GodotUtils;
global using System;

using GodotUtils.UI;

namespace Sandbox2;

public partial class Test : UIConsole
{
	public override void _Ready()
	{
		base._Ready();

		UIConsole.Test();
		UIConsole.Test2();
	}
}
