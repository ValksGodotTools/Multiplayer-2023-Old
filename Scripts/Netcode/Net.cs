using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;

namespace Multiplayer;

public static class Net
{
    public static GameClient Client { get; set; } = new();
}
