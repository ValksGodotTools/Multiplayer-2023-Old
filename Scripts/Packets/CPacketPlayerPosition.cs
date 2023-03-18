namespace Sandbox2;

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
