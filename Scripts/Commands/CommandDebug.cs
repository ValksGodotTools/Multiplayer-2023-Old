namespace Multiplayer;

public class CommandDebug : Command
{
    public override void Run(string[] args)
    {
        Logger.Log(GameMaster.OtherPlayers.PrintFull());
    }
}
