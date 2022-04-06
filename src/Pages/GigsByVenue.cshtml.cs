namespace GigLocal.Pages;

public class GigsByVenueModel : PageModel
{
    private readonly GigContext _context;

    public IEnumerable<GigByVenueRecord> Gigs { get; set; }

    public string VenueName { get; set; }

    public GigsByVenueModel(GigContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(string venueName, int? venueId)
    {
        if (venueId is null)
        {
            return NotFound();
        }

        var venue = await _context.Venues
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(v => v.ID == venueId);
        if (venue is null)
        {
            return NotFound();
        }

        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(14);
        var gigsQuery = _context.Gigs
                                .AsNoTracking()
                                .Include(g => g.Venue)
                                .Where(g => g.Approved
                                            && g.Date >= startDate
                                            && g.Date <= endDate
                                            && g.Venue.ID == venueId)
                                .OrderBy(g => g.Date);

        var gigs = await gigsQuery.ToArrayAsync();

        Gigs = gigsQuery.Select(g => new GigByVenueRecord(
            g.Date.ToDayOfWeekDateMonthName(),
            g.Date.ToTimeHourMinuteAmPm(),
            g.ArtistName,
            g.Description,
            g.EventUrl,
            g.ImageUrl
            ));

        VenueName = VenueHelper.GetFormattedNameLocation(venue.Name, venue.Suburb, venue.State);

        return Page();
    }
}

public record GigByVenueRecord
(
    string Date,
    string Time,
    string ArtistName,
    string Description,
    string EventUrl,
    string Image
);
