namespace Kootam.Framework.Utilities.Exceptions
{
    public class ArgOutOfRangeException : ArgumentOutOfRangeException
    {
        public ArgOutOfRangeException() 
        {

        }

        public ArgOutOfRangeException(string propertyName) : base($"{propertyName}مقدار نادرست می باشد.")
        {

        }
    }
}
