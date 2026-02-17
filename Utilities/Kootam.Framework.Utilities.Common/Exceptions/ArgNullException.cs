namespace Kootam.Framework.Utilities.Common.Exceptions
{
    public class ArgNullException : ArgumentNullException
    {
        public ArgNullException() : base("پارامتر نمی تواند خالی باشد")
        {

        }

        public ArgNullException(string propertyName) : base($"{propertyName}نمی تواند خالی باشد.")
        {

        }
    }
}
