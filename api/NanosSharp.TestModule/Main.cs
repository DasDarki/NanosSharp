using NanosSharp.API;

namespace NanosSharp.TestModule;

public class Main : IModule
{
    public string Name => "TestModule";
    public string Version => "1.0.0";
    
    public void Initialize(ILuaVM vm)
    {
        vm.PushGlobalTable();
        vm.GetField(-1, "print");
        vm.PushString("Hello World!");
        vm.MCall(1, 0);
        vm.Pop();

        /*vm.PushGlobalTable();
        vm.NewTable();

        vm.PushManagedFunction(ReverseString);
        vm.SetField(-2, "ReverseString");
        
        vm.SetField(-2, "TestModule");
        vm.Pop();*/
    }

    public int ReverseString(ILuaVM vm)
    {
        string s = vm.ToString(1);
        Console.WriteLine(s);
        vm.ClearStack();
        
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        
        vm.PushString(new string(charArray));
        return 1;
    }
}