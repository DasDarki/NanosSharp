#if !CLIENT
using System.Security.Cryptography;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Lightweight IL obfuscation for precompiled client assemblies: every string literal
/// is encrypted in the metadata and decrypted at runtime through an injected helper, so
/// a decompiled assembly no longer exposes its literals in the clear. Types, members and
/// attributes are left untouched, because the sandbox discovers package entry points,
/// services, APIs and entity factories by reflection and renaming would break that.
/// </summary>
internal static class ClientAssemblyObfuscator
{
    private const string HelperTypeName = "<NanosSharpStrings>";
    private const string HelperMethodName = "D";

    /// <summary>Returns the obfuscated assembly, or the original image when obfuscation is not possible.</summary>
    public static byte[] Obfuscate(byte[] image)
    {
        using var input = new MemoryStream(image);
        var assembly = AssemblyDefinition.ReadAssembly(input);
        var module = assembly.MainModule;
        var key = RandomNumberGenerator.GetInt32(1, 0x10000);

        var decrypt = InjectDecryptMethod(module, key);
        EncryptStringLiterals(module, decrypt, key);

        using var output = new MemoryStream();
        assembly.Write(output);
        return output.ToArray();
    }

    private static MethodDefinition InjectDecryptMethod(ModuleDefinition module, int key)
    {
        var characters = new ArrayType(module.TypeSystem.Char);
        var helper = new TypeDefinition(string.Empty, HelperTypeName,
            TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Class,
            module.TypeSystem.Object);
        module.Types.Add(helper);

        var decrypt = new MethodDefinition(HelperMethodName,
            MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig, module.TypeSystem.String);
        decrypt.Parameters.Add(new ParameterDefinition(module.TypeSystem.String));
        helper.Methods.Add(decrypt);

        var toCharArray = new MethodReference("ToCharArray", characters, module.TypeSystem.String) { HasThis = true };
        var fromCharacters = new MethodReference(".ctor", module.TypeSystem.Void, module.TypeSystem.String) { HasThis = true };
        fromCharacters.Parameters.Add(new ParameterDefinition(characters));

        var body = decrypt.Body;
        body.InitLocals = true;
        body.MaxStackSize = 8;
        body.Variables.Add(new VariableDefinition(characters));
        body.Variables.Add(new VariableDefinition(module.TypeSystem.Int32));
        var il = body.GetILProcessor();

        var condition = il.Create(OpCodes.Ldloc_1);
        var loop = il.Create(OpCodes.Ldloc_0);

        il.Append(il.Create(OpCodes.Ldarg_0));
        il.Append(il.Create(OpCodes.Callvirt, toCharArray));
        il.Append(il.Create(OpCodes.Stloc_0));
        il.Append(il.Create(OpCodes.Ldc_I4_0));
        il.Append(il.Create(OpCodes.Stloc_1));
        il.Append(il.Create(OpCodes.Br, condition));
        il.Append(loop);
        il.Append(il.Create(OpCodes.Ldloc_1));
        il.Append(il.Create(OpCodes.Ldloc_0));
        il.Append(il.Create(OpCodes.Ldloc_1));
        il.Append(il.Create(OpCodes.Ldelem_U2));
        il.Append(il.Create(OpCodes.Ldc_I4, key));
        il.Append(il.Create(OpCodes.Xor));
        il.Append(il.Create(OpCodes.Conv_U2));
        il.Append(il.Create(OpCodes.Stelem_I2));
        il.Append(il.Create(OpCodes.Ldloc_1));
        il.Append(il.Create(OpCodes.Ldc_I4_1));
        il.Append(il.Create(OpCodes.Add));
        il.Append(il.Create(OpCodes.Stloc_1));
        il.Append(condition);
        il.Append(il.Create(OpCodes.Ldloc_0));
        il.Append(il.Create(OpCodes.Ldlen));
        il.Append(il.Create(OpCodes.Conv_I4));
        il.Append(il.Create(OpCodes.Blt, loop));
        il.Append(il.Create(OpCodes.Ldloc_0));
        il.Append(il.Create(OpCodes.Newobj, fromCharacters));
        il.Append(il.Create(OpCodes.Ret));
        return decrypt;
    }

    private static void EncryptStringLiterals(ModuleDefinition module, MethodDefinition decrypt, int key)
    {
        foreach (var type in AllTypes(module))
        {
            foreach (var method in type.Methods)
            {
                if (method == decrypt || !method.HasBody)
                {
                    continue;
                }

                var il = method.Body.GetILProcessor();
                foreach (var instruction in method.Body.Instructions.ToArray())
                {
                    if (instruction.OpCode != OpCodes.Ldstr || instruction.Operand is not string value || value.Length == 0)
                    {
                        continue;
                    }

                    instruction.Operand = Encrypt(value, key);
                    il.InsertAfter(instruction, il.Create(OpCodes.Call, decrypt));
                }
            }
        }
    }

    private static string Encrypt(string value, int key)
    {
        var characters = value.ToCharArray();
        for (var index = 0; index < characters.Length; index++)
        {
            characters[index] = (char)(characters[index] ^ key);
        }

        return new string(characters);
    }

    private static IEnumerable<TypeDefinition> AllTypes(ModuleDefinition module)
    {
        var pending = new Stack<TypeDefinition>(module.Types);
        while (pending.Count > 0)
        {
            var type = pending.Pop();
            yield return type;
            foreach (var nested in type.NestedTypes)
            {
                pending.Push(nested);
            }
        }
    }
}
#endif
