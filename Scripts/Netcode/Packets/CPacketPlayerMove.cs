namespace Multiplayer;

public class CPacketPlayerMove : ClientPacket
{
    public Direction Horizontal { get; set; }
    public Direction Vertical   { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)Horizontal);
        writer.Write((byte)Vertical);
    }

    public override void Read(PacketReader reader)
    {
        Horizontal = (Direction)reader.ReadByte();
        Vertical   = (Direction)reader.ReadByte();
    }

    public override void Handle(Peer peer)
    {
		var player = Net.Server.Players[peer.ID];
        var dir = player.Direction;
        
        if (Horizontal == Direction.Left)
            dir.X = -1;
        else if (Horizontal == Direction.Right)
            dir.X = 1;
        else
            dir.X = 0;

        if (Vertical == Direction.Down)
            dir.Y = 1;
        else if (Vertical == Direction.Up)
            dir.Y = -1;
        else
            dir.Y = 0;

        player.Direction = dir;

        Simulation.Enqueue(GetType(), peer.ID, dir);
    }
}
