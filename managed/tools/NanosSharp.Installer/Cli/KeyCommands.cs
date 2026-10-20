using System.Security.Cryptography;
using NanosSharp.Updater;

namespace NanosSharp.Installer.Cli;

internal static class KeyCommands
{
    public static int Keygen(string? outPath)
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var publicKey = Convert.ToBase64String(key.ExportSubjectPublicKeyInfo());
        if (outPath is null)
        {
            Console.WriteLine("Private key (store as the GitHub secret RELEASE_SIGNING_KEY):");
            Console.WriteLine(key.ExportPkcs8PrivateKeyPem());
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath))!);
            File.WriteAllText(outPath, key.ExportPkcs8PrivateKeyPem() + "\n");
            Console.WriteLine($"Private key written to {outPath}");
        }

        Console.WriteLine();
        Console.WriteLine("Public key (paste into ReleaseSignature.PublicKey):");
        Console.WriteLine(publicKey);
        return 0;
    }

    public static int Sign(string manifestPath, string privateKeyPath)
    {
        var signature = ReleaseSignature.Sign(File.ReadAllBytes(manifestPath), File.ReadAllText(privateKeyPath));
        File.WriteAllBytes(manifestPath + ".sig", signature);
        Terminal.Success($"Signed {Path.GetFileName(manifestPath)}.");
        return 0;
    }
}
