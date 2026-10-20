public static class Shared
{
    public const int ProtocolVersion = 1;

#if SERVER
    public const string Side = "server";
#elif CLIENT
    public const string Side = "client";
#endif
}
