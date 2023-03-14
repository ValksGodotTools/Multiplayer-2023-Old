global using Godot;
global using GodotUtils;
global using System;

namespace Sandbox2;

public partial class Main : Node
{
	public override void _Ready()
	{
		try 
		{
			var arr = new int[2];
			arr[2] = 1;
		} catch (Exception e)
		{
			Logger.Log(e, ConsoleColor.Green);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Logger.Update();
	}
}
