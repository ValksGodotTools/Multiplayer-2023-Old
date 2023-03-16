namespace Sandbox2;

public class GameServer : ENetGameServer<PlayerData>
{
    public GameServer()
    {
        UpdateTimer.SetDelay(1000);
    }

    protected override void UpdatePlayer(KeyValuePair<uint, PlayerData> player)
    {
        // Get all the player positions except for 'player'
        var otherPlayerPositions = GetOtherPlayers(player.Key)
            .ToDictionary(x => x.Key, x => x.Value.Position);

        // No other players in the server, don't send anything
        if (otherPlayerPositions.Count == 0)
            return;

        // Send the 'other player positions' to this player
        Send(new SPacketPlayerPositions
        {
            PlayerPositions = otherPlayerPositions
        }, Peers[player.Key]);
    }
}
