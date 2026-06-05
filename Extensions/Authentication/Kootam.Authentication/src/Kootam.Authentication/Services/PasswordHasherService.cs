using System.Security.Cryptography;
using Kootam.Authentication.Abstractions.Services;
using Kootam.Authentication.Contracts.Options;
using Microsoft.Extensions.Options;

namespace Kootam.Authentication.Services;

public class PasswordHasherService(IOptions<PasswordHasherOptions> options) : IPasswordHasherService
{
    private readonly PasswordHasherOptions _options = options.Value;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        byte[] salt = GenerateSalt();
        byte[] hash = ComputeHash(password, salt);

        return EncodeHash(salt, hash);

    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        if (string.IsNullOrEmpty(hashedPassword))
            throw new ArgumentNullException(nameof(hashedPassword));

        try
        {
            var (salt, expectedHash) = DecodeHash(hashedPassword);
            byte[] actualHash = ComputeHash(password, salt);

            return SlowEquals(expectedHash, actualHash);
        }
        catch
        {
            return false;
        }
    }


    #region private method
    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[_options.SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private byte[] ComputeHash(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            _options.Iterations,
            _options.HashAlgorithm))
        {
            return pbkdf2.GetBytes(_options.HashSize);
        }
    }

    private string EncodeHash(byte[] salt, byte[] hash)
    {
        byte[] combined = new byte[1 + 4 + _options.SaltSize + _options.HashSize];

        combined[0] = 0x01; // Version
        BitConverter.GetBytes(_options.Iterations).CopyTo(combined, 1);
        salt.CopyTo(combined, 5);
        hash.CopyTo(combined, 5 + _options.SaltSize);

        return Convert.ToBase64String(combined);
    }

    private (byte[] salt, byte[] hash) DecodeHash(string hashedPassword)
    {
        byte[] combined = Convert.FromBase64String(hashedPassword);

        if (combined[0] != 0x01)
            throw new InvalidOperationException("Unsupported hash version");

        int iterations = BitConverter.ToInt32(combined, 1);

        byte[] salt = new byte[_options.SaltSize];
        Array.Copy(combined, 5, salt, 0, _options.SaltSize);

        byte[] hash = new byte[_options.HashSize];
        Array.Copy(combined, 5 + _options.SaltSize, hash, 0, _options.HashSize);

        return (salt, hash);
    }

    private bool SlowEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        uint diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }

        return diff == 0;
    }
    #endregion
}

