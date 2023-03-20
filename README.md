# Multiplayer Template
A starting multiplayer template to be used across all multiplayer games.

Ideally having all the code defined in [GodotUtils](https://github.com/Valks-Games/GodotUtils) instead of this repository would be very nice but that does not seem possible because for example `PlayerData` is a data type that is unique per game. Since `GameServer` and `GameClient` make use of `PlayerData`, they too cannot be defined in the GodotUtils library.

https://user-images.githubusercontent.com/6277739/226243215-fa5b2267-3d18-4796-8ca5-913bf5a3f135.mp4

## Table of Contents
1. [Code](#code)
2. [Setup](#setup)
3. [Contributing](#contributing)

## Code
Start up the server and client
```cs
private void _on_start_server_pressed()
{
    var ignoredPackets = new Type[]
    {
        typeof(CPacketPlayerPosition)
    };

    Net.Server.Start(25565, 100, ignoredPackets);
}

private async void _on_connect_client_pressed()
{
    var ignoredPackets = new Type[]
    {
        typeof(SPacketPlayerPositions),
    };

    Net.Client.Connect("localhost", 25565, ignoredPackets);

    while (!Net.Client.IsConnected)
        await Task.Delay(1);

    Net.Client.Send(new CPacketPlayerJoin { Username = "Fred" });
}
```

Example of a client packet
```cs
public class CPacketPlayerJoin : APacketClient
{
    public string Username { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write(Username);
    }

    public override void Read(PacketReader reader)
    {
        Username = reader.ReadString();
    }

    public override void Handle(Peer peer)
    {
        // A new player has joined the server
        Net.Server.Log($"Player {Username} (ID = {peer.ID}) joined");

        // Add this player to the server
        Net.Server.Players.Add(peer.ID, new PlayerData
        {
            Username = Username
        });

        // Tell the joining player about peer id
        Net.Server.Send(new SPacketPeerId
        {
            Id = peer.ID
        }, peer);

        // Tell the joining player about all the other players in the server
        var otherPlayers = Net.Server.GetOtherPlayers(peer.ID);
        foreach (var player in otherPlayers)
        {
            Net.Server.Send(new SPacketPlayerJoin
            {
                Id = player.Key,
                Position = new Vector2(500, 500)
            }, peer);
        }

        // Broadcast to everyone (except the joining player) about the joining player
        foreach (var player in otherPlayers)
        {
            Net.Server.Send(new SPacketPlayerJoin
            {
                Id = peer.ID,
                Position = new Vector2(500, 500)
            }, Net.Server.Peers[player.Key]);
        }
    }
}
```

Example of a server packet
```cs
public class SPacketPlayerPositions : APacketServer
{
    public Dictionary<uint, Vector2> PlayerPositions { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)PlayerPositions.Count);
        foreach (var player in PlayerPositions)
        {
            writer.Write((uint)player.Key);
            writer.Write((Vector2)player.Value);
        }
    }

    public override void Read(PacketReader reader)
    {
        var count = reader.ReadByte();
        PlayerPositions = new();
        for (int i = 0; i < count; i++)
        {
            var id = reader.ReadUInt();
            var pos = reader.ReadVector2();
            PlayerPositions.Add(id, pos);
        }
    }

    public override void Handle()
    {
        foreach (var playerPos in PlayerPositions)
        {
            GameMaster.UpdatePlayerPosition(playerPos.Key, playerPos.Value);
        }
    }
}
```

## Setup
Clone project with modules
```
git clone --recursive https://github.com/Valks-Games/Sandbox2
```

Update submodules
```
git submodule update --init --recursive
```

This requires the latest Godot 4 C# build.

## Contributing
All contributions are very much welcomed. Please contact me over Discord (`va#9904`) before you start working on something so I can give you the go ahead or say that that is something I'm currently working on and here are some other things you can work on instead.

[Projects current issues](https://github.com/Valks-Games/Multiplayer-Template/issues)
