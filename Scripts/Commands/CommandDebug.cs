namespace Sandbox2;

public class CommandDebug : Command
{
    public override void Run(string[] args)
    {
        Logger.Log(Net.Server.Players.PrintFull());
    }
}
