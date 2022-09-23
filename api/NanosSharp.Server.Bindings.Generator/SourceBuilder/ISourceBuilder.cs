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
    }
}