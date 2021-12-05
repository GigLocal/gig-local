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
        Decimal TicketPrice,
        string TicketWebsite,
        string ArtistName,
        string ArtistDescription,
        string ArtistWebsite,
        string ArtistImage,
        string VenueName,
        string VenueWebsite,
        string VenueAddress
    );

    public void OnGet()
    {
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(14);
        var gigsQuery = _context.Gigs
            .AsNoTracking()
            .Include(g => g.Artist)
            .Include(g => g.Venue)
            .Where(g => g.Date >= startDate
                        && g.Date <= endDate
                        && EF.Functions.Like(g.Venue.Address, $"%, Northcote VIC%"))
            .OrderBy(g => g.Date)
            .Take(50)
            .ToArray();

        Gigs = gigsQuery.Select(g => new GigRecord(
            g.Date.ToDayOfWeekDateMonthName(),
            g.Date.ToTimeHourMinuteAmPm(),
            g.TicketPrice,
            g.TicketWebsite,
            g.Artist.Name,
            g.Artist.Description.Truncate(200),
            g.Artist.Website,
            g.Artist.ImageUrl,
            g.Venue.Name,
            g.Venue.Website,
            g.Venue.Address));
    }
}
