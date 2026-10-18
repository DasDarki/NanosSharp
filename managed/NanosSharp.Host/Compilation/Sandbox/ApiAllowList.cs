namespace NanosSharp.Host.Compilation.Sandbox;

internal sealed class ApiAllowList
{
    private const string ClientResourceName = "NanosSharp.Host.Compilation.Sandbox.client-allowlist.txt";
    private const string NamespaceWildcard = ".*";
    private const string MemberSeparator = "::";

    public static ApiAllowList Client { get; } = Load(ClientResourceName);

    private readonly List<string> _allowedNamespaces = [];
    private readonly HashSet<string> _allowedTypes = new(StringComparer.Ordinal);
    private readonly HashSet<string> _allowedMembers = new(StringComparer.Ordinal);
    private readonly List<string> _deniedNamespaces = [];
    private readonly HashSet<string> _deniedTypes = new(StringComparer.Ordinal);
    private readonly HashSet<string> _deniedMembers = new(StringComparer.Ordinal);

    public bool IsTypeAllowed(string typeName) =>
        !IsDenied(typeName, null) && (MatchesNamespace(_allowedNamespaces, typeName) || _allowedTypes.Contains(typeName) ||
                                      _allowedMembers.Any(member => member.StartsWith(typeName + MemberSeparator, StringComparison.Ordinal)));

    public bool IsMemberAllowed(string typeName, string memberName) =>
        !IsDenied(typeName, memberName) && (MatchesNamespace(_allowedNamespaces, typeName) || _allowedTypes.Contains(typeName) ||
                                            _allowedMembers.Contains(typeName + MemberSeparator + memberName));

    private bool IsDenied(string typeName, string? memberName) =>
        MatchesNamespace(_deniedNamespaces, typeName) || _deniedTypes.Contains(typeName) ||
        (memberName is not null && _deniedMembers.Contains(typeName + MemberSeparator + memberName));

    private static bool MatchesNamespace(List<string> namespaces, string typeName) =>
        namespaces.Any(name => typeName.StartsWith(name + ".", StringComparison.Ordinal));

    private static ApiAllowList Load(string resourceName)
    {
        using var stream = typeof(ApiAllowList).Assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"The embedded allow list {resourceName} is missing.");
        using var reader = new StreamReader(stream);
        var allowList = new ApiAllowList();
        while (reader.ReadLine() is { } line)
        {
            allowList.AddRule(line.Trim());
        }

        return allowList;
    }

    private void AddRule(string rule)
    {
        if (rule.Length < 2 || rule[0] is not ('+' or '-'))
        {
            return;
        }

        var isAllowed = rule[0] == '+';
        var target = rule[1..];
        if (target.EndsWith(NamespaceWildcard, StringComparison.Ordinal))
        {
            (isAllowed ? _allowedNamespaces : _deniedNamespaces).Add(target[..^NamespaceWildcard.Length]);
            return;
        }

        if (target.Contains(MemberSeparator, StringComparison.Ordinal))
        {
            (isAllowed ? _allowedMembers : _deniedMembers).Add(target);
            return;
        }

        (isAllowed ? _allowedTypes : _deniedTypes).Add(target);
    }
}
