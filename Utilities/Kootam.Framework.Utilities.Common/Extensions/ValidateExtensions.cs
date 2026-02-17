namespace Kootam.Framework.Utilities.Common.Extensions
{
    public static class ValidateExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        public static string IsNullOrEmpty(this string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value;

        }


        public static long ToLong(this string value)
        {
            if (value.IsNullOrEmpty()) return long.MinValue;

            return Convert.ToInt64(value);

        }
    }
}
