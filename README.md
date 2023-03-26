# Multiplayer Template
A starting multiplayer template to be used across all multiplayer games.

Todo:
- Stop trying to re-invent the wheel for server simulation. Instead pump all the data to the Godot thread and do the simulation on the Godot thread. Then pump the simulation data back to the server thread. Of course all simulated objects will have to simulated on a separate collision layer.
- Experiment with client-side prediction more. Instead of lerping from prev to cur. Just lerp from cur to received. Also maybe send ID along with position packet so you know exactly when the positon occured. Because if you have ID you can get time it took for it to be send and be received and based on that you can dynamically calculate the correct `t` value for lerping. I think.

Achieving multiplayer with [ENet-CSharp](https://github.com/SoftwareGuy/ENet-CSharp) in any game is a big challenge, especially when you have to re-invent the wheel. This multiplayer template along with the GodotUtils it is using aims to solve that problem. There will still be a lot of [code](#code) you have to write but not as much as you would have to if you were doing this all by yourself.

This is by no means a production ready template as there are issues that still need to be squashed and lots of more testing to be done. More features are planned such as syncing chatting, inventory management, and NPC dialogue. But these features seem really far away as the current issues are holding me back.

Ideally having all the code defined in [GodotUtils](https://github.com/Valks-Games/GodotUtils) instead of this repository would be very nice but that does not seem possible because for example `PlayerData` is a data type that is unique per game. Since `GameServer` and `GameClient` make use of `PlayerData`, they too cannot be defined in the GodotUtils library. If you can spot something that should be defined in GodotUtils instead of this repository please tell me!

I have created a Discord specifically for this project, you can find it here https://discord.gg/dGxUw5gGgJ

You have to think about many things when setting up multiplayer.
- Should the server be in a separate repository from the client?
- Will you compensate for lag compensation? For example someone with a sniper rifle shoots someone on their screen but the server is already ahead and this client is behind. So should the client send a packet with the angle they shot at with the exact timestamp they shot at and send it to the server? Then the server can reconstruct the simulation at that point in time and see if it was valid or not. Of course you don't want the client sending timestamps that are 10 seconds old. So perhaps timestamps will only be valid if they are at most 1 or even 2 seconds old.
- How are you going to make sure `t` is synced client-side when lerping between the 2nd most recent position and most recent position to display the other clients on each of the clients?
- Should client position packets be client or server authorative. Client auth means just send raw position. No server simulation. Server auth would mean client just sends inputs and server simulates everything.
- How are you going to handle error margins. Say the client position is off by some offset from the true server position. You're going to have to slowly lerp to the correct position over time.

In other words you have 2 choices to make for each topic. Each choice has its own benefits and downfalls. But you will never truly know the downfalls if you do not try them yourselves. So my advice, try everything until you find something you like. For example an authorative client means less load on the server and no restrictive movement from the server but there is ample oportunity to send false data to the server to cheat. On the other hand a authorative server means server has to simulate everything and restrictive moement on the clients but impossible for blatant cheating.

For this template I aim to go for everything to be server authorative. The server will be separate from the client. I will look into lag compensation but I may not do it as this isn't a re-creation of CS:GO. Not sure how I will be able to improve the client-side prediction for displaying other clients. And the clients will lerp to whatever the server says the clients true position is. Of course this will make gameplay feel restrictive, I will just have to experiment with `t` a lot.

https://user-images.githubusercontent.com/6277739/226524807-214d2ec3-1197-4f93-9ebb-11d5c81537b2.mp4  

Showcasing 4 client-authorative positions being syncronized

https://user-images.githubusercontent.com/6277739/227403319-3a080d97-c801-4fd8-9e68-374e7f8e50bb.mp4  

Connecting 50 clients takes around 50 seconds. Edit: This was solved. Reason is because default min task pool limit is set to 11. Setting it to 100 and boom instant 100 clients connected.

## Table of Contents
1. [Code](#code)
2. [Setup](#setup)
3. [Contributing](#contributing)

## Code
Please note that the following code examples will most likely not be up-to-date.

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

You do not need to export the project just to get other instances running when testing the multiplayer. Instead you can go to `Godot > Debug > Run Multiple Instances -> Run X Instances`. This saves a ton of time.

## Contributing
All contributions are very much welcomed. Please contact me over Discord (`va#9904`) before you start working on something so I can give you the go ahead or say that that is something I'm currently working on and here are some other things you can work on instead.

[Projects current issues](https://github.com/Valks-Games/Multiplayer-Template/issues)
