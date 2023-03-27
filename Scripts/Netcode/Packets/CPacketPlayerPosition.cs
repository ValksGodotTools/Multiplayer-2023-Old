namespace Multiplayer;

/*
 * This packet is no longer being used as everything is going to be
 * server authorative. This means the client should send their inputs
 * instead of absolute positions.
 */
public class CPacketPlayerPosition : ClientPacket
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
        //Net.Server.Players[peer.ID].PrevCurPosition.Add(Position);
    }
}
