global using Godot;
global using GodotUtils;
global using ENet;
global using System;

namespace Sandbox2;

// NuGet Package: https://www.nuget.org/packages/ValksGodotUtils/
// No longer being maintained or used but kept here just in case

public partial class Main : Node
{
    public override async void _PhysicsProcess(double delta)
    {
        await Logger.Update();
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
            if (Net.Client != null)
            {
                Net.Client.Stop();

                while (Net.Client.IsRunning)
                    await Task.Delay(1);
            }

            if (Net.Server != null)
            {
                Net.Server.Stop();

                while (Net.Server.IsRunning)
                    await Task.Delay(1);
            }

            ENet.Library.Deinitialize();
        }
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
