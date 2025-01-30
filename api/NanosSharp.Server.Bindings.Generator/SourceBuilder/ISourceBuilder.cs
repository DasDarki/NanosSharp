namespace NanosSharp.Server.Bindings.Generator.SourceBuilder
{
    /// <summary>
    /// The source builder combining all source builders into one type.
    /// </summary>
    internal interface ISourceBuilder
    {
        /// <summary>
        /// Generates the C# source code for this builder.
        /// </summary>
        string Generate(int indent = 0);
        
        /// <summary>
        /// Checks if the given name is a reserved C# keyword and if so, makes it safe in a C# context to use.
        /// </summary>
        static string MakeSafeName(string name)
        {
            return IsReservedKeyword(name) ? $"@{name}" : name;
        }

        /// <summary>
        /// Checks if the given name is a reserved C# keyword.
        /// </summary>
        static bool IsReservedKeyword(string name)
        {
            return name switch
            {
                "abstract" => true,
                "as" => true,
                "base" => true,
                "bool" => true,
                "break" => true,
                "byte" => true,
                "case" => true,
                "catch" => true,
                "char" => true,
                "checked" => true,
                "class" => true,
                "const" => true,
                "continue" => true,
                "decimal" => true,
                "default" => true,
                "delegate" => true,
                "do" => true,
                "double" => true,
                "else" => true,
                "enum" => true,
                "event" => true,
                "explicit" => true,
                "extern" => true,
                "false" => true,
                "finally" => true,
                "fixed" => true,
                "float" => true,
                "for" => true,
                "foreach" => true,
                "goto" => true,
                "if" => true,
                "implicit" => true,
                "in" => true,
                "int" => true,
                "interface" => true,
                "internal" => true,
                "is" => true,
                "lock" => true,
                "long" => true,
                "namespace" => true,
                "new" => true,
                "null" => true,
                "object" => true,
                "operator" => true,
                "out" => true,
                "override" => true,
                "params" => true,
                "private" => true,
                "protected" => true,
                "public" => true,
                "readonly" => true,
                "ref" => true,
                "return" => true,
                "sbyte" => true,
                "sealed" => true,
                "short" => true,
                "sizeof" => true,
                "stackalloc" => true,
                "static" => true,
                "string" => true,
                "struct" => true,
                "switch" => true,
                "this" => true,
                "throw" => true,
                "true" => true,
                "try" => true,
                "typeof" => true,
                "uint" => true,
                "ulong" => true,
                "unchecked" => true,
                "unsafe" => true,
                "ushort" => true,
                "using" => true,
                "virtual" => true,
                "void" => true,
                "volatile" => true,
                "while" => true,
                _ => false
            };
        }
    }
}