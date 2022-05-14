namespace GigLocal;

public class FutureDate : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        DateTime d = Convert.ToDateTime(value);
        var now = DateTime.Now;
        var nowDate = new DateTime(now.Year, now.Month, now.Day);
        var futureDate = new DateTime(d.Year, d.Month, d.Day);
        return futureDate >= nowDate;
    }
}

public class AustralianTimeZone : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(value.ToString());
            return timeZone.Id.StartsWith("Australia");
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
    }
}
