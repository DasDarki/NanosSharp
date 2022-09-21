using NanosSharp.API;

namespace NanosSharp.TestModule;

public class Main : IModule
{
    public void Initialize(ILuaVM vm)
    {
        vm.PushGlobalTable();
        vm.GetField(-1, "print");
        vm.PushString("Hello World!");
        vm.MCall(1, 0);
        vm.ClearStack();

        vm.PushManagedFunction(ReverseString);
        vm.SetGlobal("ReverseString");
        vm.ClearStack();
    }

    public int ReverseString(ILuaVM vm)
    {
        string s = vm.ToString(1);
        vm.ClearStack();
        
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        
        vm.PushString(new string(charArray));
        return 1;
    }
}