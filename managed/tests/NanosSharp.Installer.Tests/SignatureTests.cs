using System.Security.Cryptography;
using NanosSharp.Updater;

namespace NanosSharp.Installer.Tests;

public sealed class SignatureTests
{
    [Fact]
    public void ValidSignatureIsAccepted()
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var publicKey = Convert.ToBase64String(key.ExportSubjectPublicKeyInfo());
        var manifest = "hash  nanossharp-0.2.0-linux-x64.tar.gz\n"u8.ToArray();

        var signature = ReleaseSignature.Sign(manifest, key.ExportPkcs8PrivateKeyPem());

        Assert.True(ReleaseSignature.Verify(manifest, signature, publicKey));
    }

    [Fact]
    public void TamperedManifestIsRejected()
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var publicKey = Convert.ToBase64String(key.ExportSubjectPublicKeyInfo());
        var signature = ReleaseSignature.Sign("original"u8.ToArray(), key.ExportPkcs8PrivateKeyPem());

        Assert.False(ReleaseSignature.Verify("tampered"u8.ToArray(), signature, publicKey));
    }

    [Fact]
    public void SignatureFromAnotherKeyIsRejected()
    {
        using var signer = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        using var attacker = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var manifest = "release"u8.ToArray();
        var signature = ReleaseSignature.Sign(manifest, attacker.ExportPkcs8PrivateKeyPem());

        Assert.False(ReleaseSignature.Verify(manifest, signature, Convert.ToBase64String(signer.ExportSubjectPublicKeyInfo())));
    }
}
