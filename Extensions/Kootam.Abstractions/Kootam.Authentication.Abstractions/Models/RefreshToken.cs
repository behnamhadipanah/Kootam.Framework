using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Kootam.Authentication.Abstractions.Models;

public class RefreshToken<TUserKey>
{

    public Guid Id { get; set; }
    public required TUserKey UserId { get; set; }
    public required string  Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public required string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
}

