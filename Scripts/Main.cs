global using Godot;
global using GodotUtils;
global using ENet;
global using System;

namespace Sandbox2;

public partial class Main : Manager
{
    public override async void _Ready()
    {
        PreInit(Net.Server, Net.Client);

        Net.Server.Start(25565, 100);
        Net.Client.Connect("localhost", 25565);

        while (!Net.Client.IsConnected)
            await Task.Delay(1);

        Net.Client.Send(new CPacketJoin { Username = "Fred" } );
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GodotCommands.Update();
    }
}
