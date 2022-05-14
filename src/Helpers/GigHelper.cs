namespace GigLocal.Helpers;

public class GigHelper
{
    public static string GetDateTime(DateTime startDate, DateTime? endDate)
    {
        var startDateString = startDate.ToDayOfWeekDateMonthName();
        var startTime = startDate.ToTimeHourMinuteAmPm();

        if (endDate is null)
        {
            return $"{startDateString}, {startTime}";
        }

        var endTime = endDate?.ToTimeHourMinuteAmPm();

        if (startDate.Date.Equals(endDate?.Date))
        {
            return $"{startDateString}, {startTime} - {endTime}";
        }

        var endDateString = endDate?.ToDayOfWeekDateMonthName();

        return $"{startDateString}, {startTime} - {endDateString}, {endTime}";
    }

    public static string GetUrlFriendlyName(int id, string name, string venue, DateTime date)
    {
        var nameLocation = $"{name} {venue} {date.ToString("yyyyMMdd")}".ToLowerInvariant();
        var encodedNameLocation = HttpUtility.UrlEncode(nameLocation).Replace('+', '-');
        return $"/gigs/{id}/{encodedNameLocation}";
    }
}
