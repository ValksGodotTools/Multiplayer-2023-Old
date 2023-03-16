namespace Sandbox2;

public class CommandTest : Command
{
    public override void Run(string[] args)
    {
        Logger.Log("this is a test");
    }
}
