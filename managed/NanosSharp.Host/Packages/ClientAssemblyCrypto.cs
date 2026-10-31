using System.Security.Cryptography;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Cryptography for protected client assemblies. A server holds a per-server key that
/// seals the assembly at rest and in transit; the key is delivered to a connecting
/// client with an ephemeral ECDH exchange, so no secret lives in any shipped binary
/// and reverse-engineering a build never yields a key that unlocks other servers.
/// </summary>
internal static class ClientAssemblyCrypto
{
    private const int KeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public static byte[] GenerateKey() => RandomNumberGenerator.GetBytes(KeySize);

    /// <summary>Seals data with the given key as nonce || ciphertext || tag.</summary>
    public static byte[] Seal(byte[] key, byte[] plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];
        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        var sealed_ = new byte[NonceSize + ciphertext.Length + TagSize];
        nonce.CopyTo(sealed_, 0);
        ciphertext.CopyTo(sealed_, NonceSize);
        tag.CopyTo(sealed_, NonceSize + ciphertext.Length);
        return sealed_;
    }

    public static byte[] Unseal(byte[] key, byte[] sealed_)
    {
        if (sealed_.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("The sealed data is too short.");
        }

        var nonce = sealed_.AsSpan(0, NonceSize);
        var tag = sealed_.AsSpan(sealed_.Length - TagSize, TagSize);
        var ciphertext = sealed_.AsSpan(NonceSize, sealed_.Length - NonceSize - TagSize);
        var plaintext = new byte[ciphertext.Length];
        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }

    public static ECDiffieHellman CreateEphemeral() => ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

    public static byte[] ExportPublicKey(ECDiffieHellman keyPair) => keyPair.PublicKey.ExportSubjectPublicKeyInfo();

    /// <summary>
    /// Wraps a key for a recipient identified by its public key, using a fresh ephemeral
    /// key pair. Returns the ephemeral public key and the wrapped key.
    /// </summary>
    public static (byte[] SenderPublicKey, byte[] Wrapped) WrapKey(byte[] key, byte[] recipientPublicKey)
    {
        using var sender = CreateEphemeral();
        var shared = DeriveShared(sender, recipientPublicKey);
        return (ExportPublicKey(sender), Seal(shared, key));
    }

    public static byte[] UnwrapKey(byte[] wrapped, byte[] senderPublicKey, ECDiffieHellman recipient)
    {
        var shared = DeriveShared(recipient, senderPublicKey);
        return Unseal(shared, wrapped);
    }

    private static byte[] DeriveShared(ECDiffieHellman self, byte[] otherPublicKey)
    {
        using var other = ECDiffieHellman.Create();
        other.ImportSubjectPublicKeyInfo(otherPublicKey, out _);
        return self.DeriveKeyFromHash(other.PublicKey, HashAlgorithmName.SHA256);
    }
}
