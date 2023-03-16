using ENet;

namespace Sandbox2;

public class CPacketJoin : APacketClient
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
        Net.Server.Log($"Player {peer.ID} joined");

        Net.Server.Players.Add(peer.ID, new PlayerData
        {
            Username = Username
        });

        Net.Server.Send(new SPacketSpawnPlayer { Id = peer.ID }, peer);
    }
}

public class CPacketPlayerPosition : APacketClient
{
    public Vector2 Position { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write(Position);
    }

    public override void Read(PacketReader reader)
    {
        Position = reader.ReadVector2();
    }

    public override void Handle(Peer peer)
    {
        Net.Server.Players[peer.ID].Position = Position;
    }
}

public class SPacketSpawnPlayer : APacketServer
{
    public uint Id { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write(Id);
    }

    public override void Read(PacketReader reader)
    {
        Id = reader.ReadUInt();
    }

    public override void Handle()
    {
        GameMaster.CreateOtherPlayer(Id, new PlayerData
        {
            Position = new Vector2(500, 500)
        });
    }
}

public class SPacketPlayerPositions : APacketServer
{
    public Dictionary<uint, Vector2> PlayerPositions { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write(PlayerPositions.Count);
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
        
    }
}
