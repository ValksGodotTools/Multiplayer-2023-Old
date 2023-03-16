using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;

namespace Sandbox2;

public static class Net
{
    public static GameServer Server { get; set; } = new();
    public static GameClient Client { get; set; } = new();
}
