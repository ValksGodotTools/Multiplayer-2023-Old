namespace Sandbox2;

public class CommandDebug : Command
{
    public override void Run(string[] args)
    {
        if (args.Length == 0)
        {
            Logger.Log("Please specify args server or client");
        }

        if (args[0] == "server")
        {
            Logger.Log(Net.Server.Players.PrintFull());
        }

        if (args[0] == "client")
        {
            Logger.Log(GameMaster.Players.PrintFull());
        }
    }
}
