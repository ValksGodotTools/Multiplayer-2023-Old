namespace Sandbox2;

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
        var players = Net.Server.Players;

        players.Add(peer.ID, new PlayerData
        {
            Username = Username
        });

        // Tell this player that they have joined
        Net.Server.Log($"Tell this player that they have joined ID={peer.ID}", ConsoleColor.Red);
        Net.Server.Send(new SPacketPlayerJoin
        {
            Id = peer.ID
        }, peer);

        // Tell all other clients about this player excluding the player that just joined
        foreach (var player in Net.Server.GetOtherPlayers(peer.ID))
        {
            Net.Server.Send(new SPacketPlayerJoin
            {
                Id = peer.ID
            }, Net.Server.Peers[player.Key]);
        }
    }
}
