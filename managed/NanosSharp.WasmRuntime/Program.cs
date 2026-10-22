namespace NanosSharp.WasmRuntime;

internal static class Program
{
    private static int Main(string[] args) => NanosSharp.Interop.Wasm.WasmBootstrap.Run(args);
}
