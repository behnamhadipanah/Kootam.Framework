using System.Globalization;

namespace Kootam.Framework.Utilities.Common.Extensions;

public static class DateExtensions
{
    public static string ConvertToPersianDate(this DateTime date, string? splitWith, bool? hasClock = false)
    {
        if (splitWith.IsNullOrEmpty()) splitWith = "-";
        PersianCalendar pc = new PersianCalendar();

        if (hasClock.HasValue && hasClock.Value)
            return
                $"{pc.GetYear(date)}{splitWith}{pc.GetMonth(date):D2}{splitWith}{pc.GetDayOfMonth(date):D2} {date.Hour:D2}:{date.Minute:D2}:{date.Second:D2}";

        return $"{pc.GetYear(date)}{splitWith}{pc.GetMonth(date):D2}{splitWith}{pc.GetDayOfMonth(date):D2}";
    }

    public static DateTime ConvertToGregorianDate(this string date, bool? hasClock = false)
    {
        if (string.IsNullOrEmpty(date))
            throw new Exception("Date cannot be null or empty");


        string[] datepart = date.Split(new char[] { '/', '-' });
        if (datepart.Length != 3)
            throw new Exception("Date is not valid");
        int year = 0, month = 0, day = 0;
        DateTime gregorianDate;

        year = datepart[0].Length == 4 ? int.Parse(datepart[0]) : int.Parse(datepart[2]);
        month = int.Parse(datepart[1]);
        day = datepart[0].Length == 4 ? int.Parse(datepart[2]) : int.Parse(datepart[0]);
        var currentCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        gregorianDate = new DateTime(year, month, day, new PersianCalendar());
        if (hasClock.HasValue && hasClock.Value) return gregorianDate;
        
        return gregorianDate.Date;
    }
}