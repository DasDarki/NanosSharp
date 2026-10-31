using System.Security.Cryptography;
using NanosSharp.Host.Packages;

namespace NanosSharp.Host.Tests;

public class ClientAssemblyCryptoTests
{
    [Fact]
    public void SealRoundTripsWithTheSameKey()
    {
        var key = ClientAssemblyCrypto.GenerateKey();
        var data = RandomNumberGenerator.GetBytes(4096);

        var sealed_ = ClientAssemblyCrypto.Seal(key, data);

        Assert.NotEqual(data, sealed_);
        Assert.Equal(data, ClientAssemblyCrypto.Unseal(key, sealed_));
    }

    [Fact]
    public void UnsealWithAWrongKeyFails()
    {
        var sealed_ = ClientAssemblyCrypto.Seal(ClientAssemblyCrypto.GenerateKey(), RandomNumberGenerator.GetBytes(64));

        Assert.ThrowsAny<CryptographicException>(() => ClientAssemblyCrypto.Unseal(ClientAssemblyCrypto.GenerateKey(), sealed_));
    }

    [Fact]
    public void KeyExchangeDeliversTheKeyToTheHolderOfThePrivateKey()
    {
        var key = ClientAssemblyCrypto.GenerateKey();
        using var client = ClientAssemblyCrypto.CreateEphemeral();
        var clientPublicKey = ClientAssemblyCrypto.ExportPublicKey(client);

        var (serverPublicKey, wrapped) = ClientAssemblyCrypto.WrapKey(key, clientPublicKey);

        Assert.Equal(key, ClientAssemblyCrypto.UnwrapKey(wrapped, serverPublicKey, client));
    }

    [Fact]
    public void KeyExchangeDoesNotLeakToAnotherClient()
    {
        var key = ClientAssemblyCrypto.GenerateKey();
        using var client = ClientAssemblyCrypto.CreateEphemeral();
        using var attacker = ClientAssemblyCrypto.CreateEphemeral();

        var (serverPublicKey, wrapped) = ClientAssemblyCrypto.WrapKey(key, ClientAssemblyCrypto.ExportPublicKey(client));

        Assert.ThrowsAny<CryptographicException>(() => ClientAssemblyCrypto.UnwrapKey(wrapped, serverPublicKey, attacker));
    }
}
