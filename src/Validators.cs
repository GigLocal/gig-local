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
