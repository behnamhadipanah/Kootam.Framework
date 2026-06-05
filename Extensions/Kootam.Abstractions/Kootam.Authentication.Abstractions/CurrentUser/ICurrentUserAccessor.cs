namespace Kootam.Extensions.Authentication.Abstractions.CurrentUser;

public interface ICurrentUserAccessor
{
    TUser Get<TUser>() where TUser : class;

}
