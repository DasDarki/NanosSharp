using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NanosSharp.Host.Packages;

/// <summary>
/// Cryptography for protected client assemblies. A server holds a per-server key that
/// seals the assembly at rest and in transit; the key is delivered to a connecting
/// client with an ephemeral X25519 exchange, so no secret lives in any shipped binary
/// and reverse-engineering a build never yields a key that unlocks other servers.
/// The implementation is pure managed (BouncyCastle), because the client host runs on
/// Wine where the operating system EC key provider is unavailable.
/// </summary>
internal static class ClientAssemblyCrypto
{
    public const int KeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private static readonly SecureRandom Random = new();

    public static byte[] GenerateKey()
    {
        var key = new byte[KeySize];
        Random.NextBytes(key);
        return key;
    }

    /// <summary>Seals data with the given key as nonce || ciphertext || tag.</summary>
    public static byte[] Seal(byte[] key, byte[] plaintext)
    {
        var nonce = new byte[NonceSize];
        Random.NextBytes(nonce);

        var cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(true, new AeadParameters(new KeyParameter(key), TagSize * 8, nonce));
        var output = new byte[cipher.GetOutputSize(plaintext.Length)];
        var offset = cipher.ProcessBytes(plaintext, 0, plaintext.Length, output, 0);
        cipher.DoFinal(output, offset);

        var sealed_ = new byte[NonceSize + output.Length];
        nonce.CopyTo(sealed_, 0);
        output.CopyTo(sealed_, NonceSize);
        return sealed_;
    }

    public static byte[] Unseal(byte[] key, byte[] sealed_)
    {
        if (sealed_.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("The sealed data is too short.");
        }

        var nonce = sealed_.AsSpan(0, NonceSize).ToArray();
        var input = sealed_.AsSpan(NonceSize).ToArray();
        var cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(false, new AeadParameters(new KeyParameter(key), TagSize * 8, nonce));
        var output = new byte[cipher.GetOutputSize(input.Length)];
        var offset = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        try
        {
            cipher.DoFinal(output, offset);
        }
        catch (InvalidCipherTextException exception)
        {
            throw new CryptographicException("The sealed data could not be authenticated.", exception);
        }

        return output;
    }

    public static EphemeralKeyPair CreateEphemeral()
    {
        var generator = new X25519KeyPairGenerator();
        generator.Init(new X25519KeyGenerationParameters(Random));
        var pair = generator.GenerateKeyPair();
        var publicKey = ((X25519PublicKeyParameters)pair.Public).GetEncoded();
        return new EphemeralKeyPair((X25519PrivateKeyParameters)pair.Private, publicKey);
    }

    public static byte[] ExportPublicKey(EphemeralKeyPair keyPair) => keyPair.PublicKey;

    /// <summary>
    /// Wraps a key for a recipient identified by its public key, using a fresh ephemeral
    /// key pair. Returns the ephemeral public key and the wrapped key.
    /// </summary>
    public static (byte[] SenderPublicKey, byte[] Wrapped) WrapKey(byte[] key, byte[] recipientPublicKey)
    {
        var sender = CreateEphemeral();
        var shared = DeriveShared(sender.PrivateKey, recipientPublicKey);
        return (sender.PublicKey, Seal(shared, key));
    }

    public static byte[] UnwrapKey(byte[] wrapped, byte[] senderPublicKey, EphemeralKeyPair recipient)
    {
        var shared = DeriveShared(recipient.PrivateKey, senderPublicKey);
        return Unseal(shared, wrapped);
    }

    private static byte[] DeriveShared(X25519PrivateKeyParameters self, byte[] otherPublicKey)
    {
        var agreement = new X25519Agreement();
        agreement.Init(self);
        var secret = new byte[agreement.AgreementSize];
        agreement.CalculateAgreement(new X25519PublicKeyParameters(otherPublicKey), secret, 0);

        var digest = new Sha256Digest();
        digest.BlockUpdate(secret, 0, secret.Length);
        var derived = new byte[digest.GetDigestSize()];
        digest.DoFinal(derived, 0);
        return derived;
    }
}

/// <summary>An ephemeral X25519 key pair used for a single key exchange.</summary>
internal sealed class EphemeralKeyPair(X25519PrivateKeyParameters privateKey, byte[] publicKey)
{
    public X25519PrivateKeyParameters PrivateKey { get; } = privateKey;

    public byte[] PublicKey { get; } = publicKey;
}
