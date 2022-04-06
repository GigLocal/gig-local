namespace GigLocal.Pages;

public class GigsModel : PageModel
{
    private readonly GigContext _context;

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

    public async Task<IActionResult> OnGetAsync(string venueName, int? venueId)
    {
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(14);
        var gigsQuery = _context.Gigs
            .AsNoTracking()
            .Include(g => g.Venue)
            .Where(g => g.Approved
                        && g.Date >= startDate
                        && g.Date <= endDate);

        ViewData["Title"] = "Gigs";

        if (venueId is not null)
        {
            var venue = await _context.Venues
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(v => v.ID == venueId);
            if (venue is null)
            {
                return NotFound();
            }

            ViewData["Title"] = $"Gigs at {VenueHelper.GetFormattedNameLocation(venue.Name, venue.Suburb, venue.State)}";

            gigsQuery = gigsQuery.Where(g => g.Venue.ID == venueId);
        }

        var gigs = await gigsQuery.OrderBy(g => g.Date).ToArrayAsync();

        Gigs = gigsQuery.Select(g => new GigRecord(
            g.Date.ToDayOfWeekDateMonthName(),
            g.Date.ToTimeHourMinuteAmPm(),
            g.ArtistName,
            g.Description,
            g.EventUrl,
            g.ImageUrl,
            VenueHelper.GetFormattedNameLocation(g.Venue.Name, g.Venue.Suburb, g.Venue.State)
            ));

        return Page();
    }
}
