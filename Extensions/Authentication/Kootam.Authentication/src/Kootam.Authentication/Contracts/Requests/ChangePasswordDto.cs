namespace Kootam.Authentication.Contracts.Requests;

public record ChangePasswordDto(string CurrentPassword,string NewPassword,string ConfirmNewPassword);

