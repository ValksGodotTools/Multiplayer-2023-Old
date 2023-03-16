using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;
using Sandbox2;

namespace GodotUtils.Netcode;

public static class Net
{
    public static GameServer Server { get; set; } = new();
    public static GameClient Client { get; set; } = new();
}
