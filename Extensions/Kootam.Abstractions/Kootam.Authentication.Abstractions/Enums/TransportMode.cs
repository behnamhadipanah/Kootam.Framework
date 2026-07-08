namespace Kootam.Authentication.Abstractions.Enums;

public enum TransportMode
{
    AuthorizationHeader,
    Cookie,
    Session,
    QueryString,
    Composite
}