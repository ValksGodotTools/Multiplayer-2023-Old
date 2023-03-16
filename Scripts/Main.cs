global using Godot;
global using GodotUtils;
global using ENet;
global using System;

namespace Sandbox2;

public partial class Main : Node
{
    public static Main Instance { get; set; }

    public override async void _Ready()
    {
        Instance = this;

        Net.Server.Start(25565, 100);
        Net.Client.Connect("localhost", 25565);

        while (!Net.Client.IsConnected)
            await Task.Delay(1);

        Net.Client.Send(new CPacketJoin { Username = "Fred" } );
    }

    public override void _PhysicsProcess(double delta)
    {
        Logger.Update();
        GodotCommands.Update();
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GetTree().AutoAcceptQuit = false;
            await Cleanup();
        }
    }

    public async Task Cleanup()
    {
        await Net.Cleanup();

        if (Logger.StillWorking())
            await Task.Delay(1);

        GetTree().Quit();
    }
}
