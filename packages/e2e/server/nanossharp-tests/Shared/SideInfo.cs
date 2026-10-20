namespace NanosSharp.Tests.Shared;

public static class SideInfo
{
#if SERVER
    public const string Name = "server";
#elif CLIENT
    public const string Name = "client";
#else
    public const string Name = "unknown";
#endif
}
