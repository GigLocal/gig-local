namespace GigLocal.Pages;

public class GigsModel : PageModel
{
    private readonly GigContext _context;

    [BindProperty]
    public IEnumerable<GigRecord> Gigs { get; set; }

    public GigsModel(GigContext context)
    {
        _context = context;
    }

    public record GigRecord
    (
        string Date,
        string Time,
        string ArtistName,
        string Description,
        string EventUrl,
        string Image,
        string VenueName
    );

    public void OnGet()
    {
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(14);
        var gigsQuery = _context.Gigs
            .AsNoTracking()
            .Include(g => g.Artist)
            .Include(g => g.Venue)
            .Where(g => g.Approved
                        && g.Date >= startDate
                        && g.Date <= endDate)
            .OrderBy(g => g.Date)
            .ToArray();

        Gigs = gigsQuery.Select(g => new GigRecord(
            g.Date.ToDayOfWeekDateMonthName(),
            g.Date.ToTimeHourMinuteAmPm(),
            g.ArtistName ?? g.Artist.Name,
            g.Description ?? g.Artist.Description,
            g.EventUrl ?? g.Venue.Website,
            g.ImageUrl ?? g.Artist.ImageUrl,
            g.Venue.Name));
    }
}
