namespace Kootam.Framework.Domain.Exceptions;

public class InvalidApplicationServiceStateException : DomainStateException
{
    public InvalidApplicationServiceStateException(string message, params string[] parameters) : base(message)
    {
        Parameters = parameters;
    }
}