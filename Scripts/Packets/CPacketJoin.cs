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
