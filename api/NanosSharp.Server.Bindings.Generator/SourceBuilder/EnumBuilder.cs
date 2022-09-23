using System.Text;

namespace NanosSharp.Server.Bindings.Generator.SourceBuilder
{
    internal class EnumBuilder : ISourceBuilder
    {
        private readonly string _name;
        private readonly Dictionary<string, string> _values;

        internal EnumBuilder(string name)
        {
            _name = name;
            _values = new Dictionary<string, string>();
        }
        
        internal void AddValue(string name, string value)
        {
            _values.Add(name, value);
        }

        public string Generate(int indent = 0)
        {
            var sb = new StringBuilder();
            sb.AppendLineWithIndent($"public enum {_name}", indent).AppendLineWithIndent("{", indent);

            int i = 0;
            foreach (var e in _values)
            {
                sb.AppendLineWithIndent($"{e.Key} = {e.Value}{(i < _values.Count - 1 ? "," : "")}", indent + 1);
                i++;
            }

            sb.AppendLineWithIndent("}", indent);
            return sb.ToString();
        }
    }
}