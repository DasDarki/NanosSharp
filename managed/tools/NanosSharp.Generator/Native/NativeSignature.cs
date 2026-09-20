namespace NanosSharp.Generator.Native;

internal sealed record NativeSignature(NativeFunctionKind Kind, CppType? ReturnType, IReadOnlyList<CppType> Parameters)
{
    private const string Scripting = "nw::scripting::";

    public static NativeSignature Parse(string signature)
    {
        var type = CppTypeParser.Parse(signature);
        var arguments = type.Arguments;
        return type.Name switch
        {
            Scripting + "MemberFunction" or Scripting + "MemberFunctionCompatibility" =>
                new NativeSignature(NativeFunctionKind.Member, arguments[1], arguments.Skip(2).ToArray()),
            Scripting + "MemberStaticFunction" or Scripting + "MemberStaticFunctionCompatibility" or Scripting + "Function" =>
                new NativeSignature(NativeFunctionKind.Static, arguments[0], arguments.Skip(1).ToArray()),
            Scripting + "MemberStaticFunctionEnvironment" =>
                new NativeSignature(NativeFunctionKind.Static, arguments[0], arguments.Skip(2).ToArray()),
            _ => new NativeSignature(NativeFunctionKind.Special, null, []),
        };
    }
}
