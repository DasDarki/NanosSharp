namespace NanosSharp.Runtime.Native;

// Credits: https://github.com/nxrighthere/UnrealCLR/blob/master/Source/Managed/Framework/Codegen.cs
internal struct Bool {
    private byte _value;

    public Bool(byte value) => _value = value;

    public static implicit operator bool(Bool value) => value._value != 0;

    public static implicit operator Bool(bool value) => !value ? new Bool(0) : new Bool(1);

    public override int GetHashCode() => _value.GetHashCode();
}