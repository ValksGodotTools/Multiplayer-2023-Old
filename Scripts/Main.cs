global using Godot;
global using GodotUtils;
global using ENet;
global using System;

namespace Multiplayer;

// NuGet Package: https://www.nuget.org/packages/ValksGodotUtils/
// No longer being maintained or used but kept here just in case

public partial class Main : Node
{
    public override void _PhysicsProcess(double delta)
    {
        Logger.Update();
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GetTree().AutoAcceptQuit = false;
            await Cleanup();
        }
    }

    private async Task Cleanup()
    {
        await CleanupNet();

        if (Logger.StillWorking())
            await Task.Delay(1);

        GetTree().Quit();
    }

    private async Task CleanupNet()
    {
        if (ENetLow.ENetInitialized)
        {
            if (Net.Server != null)
            {
                Net.Server.Stop();

                while (Net.Server.IsRunning)
                    await Task.Delay(1);
            }

            if (Net.Client != null)
            {
                Net.Client.Stop();

                while (Net.Client.IsRunning)
                    await Task.Delay(1);
            }

            ENet.Library.Deinitialize();
        }
    }

    private void _on_start_server_pressed()
    {
        if (Net.Server.IsRunning)
        {
            Net.Server.Log("The server is running already");
            return;
        }

        Simulation.Start();

        var ignoredPackets = new Type[]
        {
            typeof(CPacketPlayerPosition)
        };

        Net.Server.Start(25565, 100, new ENetOptions
        {
            PrintPacketData = false,
            PrintPacketSent = false,
            PrintPacketReceived = false
        }, ignoredPackets);
    }

    private async void _on_connect_client_pressed()
    {
        if (Net.Client.IsRunning)
        {
            Net.Client.Log("The client is running already");
            return;
        }

        var ignoredPackets = new Type[]
        {
            typeof(SPacketPlayerPositions),
        };

        Net.Client.Connect("localhost", 25565, new ENetOptions
        {
            PrintPacketData = false,
            PrintPacketReceived = false,
            PrintPacketSent = false
        }, ignoredPackets);

        while (!Net.Client.IsConnected)
            await Task.Delay(1);

        Net.Client.Send(new CPacketPlayerJoin { Username = "Fred" });
    }
}
