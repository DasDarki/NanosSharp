﻿using NanosSharp.API;

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
        
        vm.PushGlobalTable();
        vm.GetField(-1, "Server");
        vm.GetField(-1, "Subscribe");
        vm.PushString("Tick");
        vm.PushManagedFunction(OnTick);
        vm.MCall(2, 0);
        vm.ClearStack();
    }

    public int OnTick(ILuaVM vm)
    {
        Console.WriteLine("Tick");
        return 0;
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