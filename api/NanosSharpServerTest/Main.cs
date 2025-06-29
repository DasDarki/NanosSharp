using NanosSharp.Server;

namespace NanosSharpServerTest;

public class Main : Package
{
    public override void OnStart()
    {
        Logger.Info("NanosSharpServerTest package started successfully.");
        Logger.Info("\tPackage directory: {X}", Directory);
    }
}