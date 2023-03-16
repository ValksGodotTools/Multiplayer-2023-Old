using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;
using Sandbox2;

namespace GodotUtils.Netcode;

public static class Net
{
    public static GameServer Server { get; set; } = new();
    public static GameClient Client { get; set; } = new();
    public static bool ENetInitialized { get; set; }

    public static async Task Cleanup()
    {
        if (ENetInitialized)
        {
            if (Client != null)
            {
                Client.Stop();

                while (Client.IsRunning)
                    await Task.Delay(1);
            }

            if (Server != null)
            {
                Server.Stop();

                while (Server.IsRunning)
                    await Task.Delay(1);
            }

            ENet.Library.Deinitialize();
        }
    }
}
