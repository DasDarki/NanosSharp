using System.Security.Cryptography;

namespace NanosSharp.Updater;

/// <summary>Verifies that a release manifest was signed with the NanosSharp release key.</summary>
public static class ReleaseSignature
{
    public const string SignatureAsset = "SHA256SUMS.sig";

    private const string PublicKey = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEIs6ySYFkVqJiPYpmrLZTa8riw6XDoeCaszmgsWFFaMoBAxoLA++aOiH5XZATI9Ws81uAdILk8u0FKhkMkujW+Q==";

    public static bool IsConfigured => PublicKey.Length > 0;

    public static bool Verify(byte[] manifest, byte[] signature) => Verify(manifest, signature, PublicKey);

    internal static bool Verify(byte[] manifest, byte[] signature, string publicKeyBase64)
    {
        using var key = ECDsa.Create();
        key.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64), out _);
        return key.VerifyData(manifest, signature, HashAlgorithmName.SHA256);
    }

    public static byte[] Sign(byte[] manifest, string privateKeyPem)
    {
        using var key = ECDsa.Create();
        key.ImportFromPem(privateKeyPem);
        return key.SignData(manifest, HashAlgorithmName.SHA256);
    }
}
