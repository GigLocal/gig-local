namespace GigLocal;

public static class Extensions
{
    public static string Truncate(this string value, int maxLength)
    {
        if (value?.Length <= maxLength)
        {
            return value;
        }
        return $"{value.Substring(0, Math.Min(value.Length, maxLength-3))}...";
    }

    public static string ToDaySuffix(this int day)
    {
        switch (day)
        {
            case 1:
            case 21:
            case 31:
                return "st";
            case 2:
            case 22:
                return "nd";
            case 3:
            case 23:
                return "rd";
            default:
                return "th";
        }
    }

    public static string ToDayOfWeekDateMonthName(this DateTime dateTime)
    {
        return $"{dateTime.ToString("dddd")} {dateTime.Day}{dateTime.Day.ToDaySuffix()} {dateTime.ToString("MMMM")}";
    }

    public static string ToTimeHourMinuteAmPm(this DateTime dateTime)
    {
        return dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
    }
}
