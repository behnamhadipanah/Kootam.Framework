namespace Kootam.Framework.Authentication.Jwt.Configurations;

public class AuthConstants
{
    public string Audiance { get; set; } = "KotamGroups";
    public string Key { get; set; } = "*L0V3_h@dip@n@h_Jwt*C0D3*G3N3r4teT0K3N*";
    public string Issuer { get; set; } = "KotamGroups";
    public string TokenPath { get; set; } = "/auth/login";

    public double ExpireTokenPerMinutes { get; set; } = 60;
    public double ExpireRefreshTokenPerMinutes { get; set; } = 30;
}

