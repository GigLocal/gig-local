namespace GigLocal.Helpers;

public class VenueHelper
{
    public static async Task<IEnumerable<SelectListItem>> GetSelectListAsync(GigContext context)
    {
        return (await context.Venues.Select(v => new {Name = v.Name, ID = v.ID})
                                        .OrderBy(v => v.Name)
                                        .ToListAsync())
                                        .Select(v => new SelectListItem(v.Name, v.ID.ToString()));
    }
}
