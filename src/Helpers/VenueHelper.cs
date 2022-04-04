namespace GigLocal.Helpers;

public class VenueHelper
{
    public static readonly IEnumerable<SelectListItem> States = new [] {
        "ACT", "NSW", "NT", "QLD", "SA", "TAS", "VIC", "WA"
    }.Select(s => new SelectListItem(s, s));

    public static async Task<IEnumerable<SelectListItem>> GetSelectListAsync(GigContext context)
    {
        return (await context.Venues.Select(v => new {Name = v.Name, ID = v.ID})
                                        .OrderBy(v => v.Name)
                                        .ToListAsync())
                                        .Select(v => new SelectListItem(v.Name, v.ID.ToString()));
    }

    public static string GetFormattedAddress(string address, string suburb, string state, int postcode)
    {
        return $"{address}, {suburb} {state} {postcode}";
    }

    public static string GetFormattedNameLocation(string name, string suburb, string state)
    {
        return $"{name}, {suburb} {state}";
    }
}
