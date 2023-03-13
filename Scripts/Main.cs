global using Godot;
global using GodotUtils;
global using System;

namespace Sandbox2;

public partial class Main : Node
{
	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
	}
}
