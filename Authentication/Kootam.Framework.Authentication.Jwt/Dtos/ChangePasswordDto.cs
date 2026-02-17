namespace Kootam.Framework.Authentication.Jwt.Dtos;

public record ChangePasswordDto(string CurrentPassword,string NewPassword,string ConfirmNewPassword);

