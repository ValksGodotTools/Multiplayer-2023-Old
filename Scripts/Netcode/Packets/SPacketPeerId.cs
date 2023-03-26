namespace Multiplayer;

/*
 * This packet class was created to tell the client their server peer ID.
 * Although this seems useless as I cannot think of a application as to why
 * the client would need to know this.
 */
public class SPacketPeerId : ServerPacket
{
    public uint Id { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((uint)Id);
    }

    public override void Read(PacketReader reader)
    {
        Id = reader.ReadUInt();
    }

    public override void Handle()
    {
        //GameMaster.PeerId = Id;
        //Net.Client.Log("My client ID is " + GameMaster.PeerId);
    }
}
