using System.Security.Cryptography;

namespace Kootam.Authentication.Contracts.Options;

public class PasswordHasherOptions
{
    public int SaltSize { get; set; } = 16;
    public int HashSize { get; set; } = 32;
    public int Iterations { get; set; } = 100000;
    public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;
}