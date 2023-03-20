namespace Sandbox2;

public class SPacketPlayerJoin : APacketServer
{
    public uint Id { get; set; }
    public Vector2 Position { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((uint)Id);
        writer.Write(Position);
    }

    public override void Read(PacketReader reader)
    {
        Id = reader.ReadUInt();
        Position = reader.ReadVector2();
    }

    public override void Handle()
    {
        GameMaster.AddPlayer(Id, Position);
    }
}
