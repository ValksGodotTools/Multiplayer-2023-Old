namespace Sandbox2;

public class SPacketPlayerJoinLeave : APacketServer
{
    public uint    Id       { get; set; }
    public bool    Joining  { get; set; }
    public Vector2 Position { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((uint)Id);
        writer.Write(Joining);

        if (Joining)
        {
            writer.Write(Position);
        }
    }

    public override void Read(PacketReader reader)
    {
        Id = reader.ReadUInt();
        Joining = reader.ReadBool();

        if (Joining)
        {
            Position = reader.ReadVector2();
        }
    }

    public override void Handle()
    {
        if (Joining)
        {
            GameMaster.AddPlayer(Id, Position);
        }
        else
        {
            GameMaster.RemovePlayer(Id);
        }
    }
}
