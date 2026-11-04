using System.Security.Cryptography;
using Kootam.Authentication.Abstractions.Services;

namespace Kootam.Authentication.SignIn.Services;

internal sealed class NumericContactOtpCodeGenerator : IContactOtpCodeGenerator
{
    public string Generate()
        => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
}
