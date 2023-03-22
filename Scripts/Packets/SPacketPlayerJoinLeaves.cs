namespace Sandbox2;

public class SPacketPlayerJoinLeaves : ServerPacket
{
    public Dictionary<uint, JoinLeaveData> Data { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)Data.Count);

        foreach (var player in Data)
        {
            var id = player.Key;
            var data = player.Value;

            writer.Write(id);
            writer.Write(data.Joining);

            if (data.Joining)
            {
                writer.Write(data.Position);
            }
        }
    }

    public override void Read(PacketReader reader)
    {
        var count = reader.ReadByte();
        Data = new();

        for (int i = 0; i < count; i++)
        {
            var id = reader.ReadUInt();
            var joining = reader.ReadBool();

            var position = Vector2.Zero;

            if (joining)
            {
                position = reader.ReadVector2();
            }

            Data.Add(id, new JoinLeaveData
            {
                Joining = joining,
                Position = position
            });
        }
    }

    public override void Handle()
    {
        foreach (var player in Data)
        {
            var id = player.Key;
            var data = player.Value;

            if (data.Joining)
            {
                GameMaster.AddPlayer(id, data.Position);
            }
            else
            {
                GameMaster.RemovePlayer(id);
            }
        }
    }
}

public class JoinLeaveData
{
    public bool Joining { get; set; }
    public Vector2 Position { get; set; }
}
