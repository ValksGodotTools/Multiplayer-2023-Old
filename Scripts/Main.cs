global using Godot;
global using GodotUtils;
global using ENet;
global using System;

namespace Sandbox2;

public partial class Main : Manager<PlayerData>
{
    public override void _Ready()
    {
        PreInit(Net.Server, Net.Client);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GodotCommands.Update();
    }

    private void _on_start_server_pressed()
    {
        Net.Server.Start(25565, 100);
    }

    private async void _on_connect_client_pressed()
    {
        Net.Client.Connect("localhost", 25565);

        while (!Net.Client.IsConnected)
            await Task.Delay(1);

        Net.Client.Send(new CPacketJoin { Username = "Fred" });
    }
}
