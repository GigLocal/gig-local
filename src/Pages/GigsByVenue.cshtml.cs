namespace GigLocal.Pages;

public class GigsByVenueModel : BasePageModel
{
    private readonly GigContext _context;

    public IEnumerable<GigByVenueRecord> Gigs { get; set; }

    public VenueRecord Venue { get; set; }

    public GigsByVenueModel(GigContext context, MetaTagService metaTagService) : base(metaTagService)
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

        var gigsQuery = _context.Gigs
                                .AsNoTracking()
                                .Include(g => g.Venue)
                                .Where(g => g.Approved
                                            && g.Date >= startDate
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

        Venue = new VenueRecord(
            venue.Name,
            VenueHelper.GetFormattedNameLocation(venue.Name, venue.Suburb, venue.State),
            VenueHelper.GetFormattedAddress(venue.Address, venue.Suburb, venue.State, venue.Postcode),
            venue.Description,
            venue.Website,
            venue.ImageUrl,
            VenueHelper.GetGoogleMapsUrl(venue.Name, venue.Address, venue.Suburb, venue.State, venue.Postcode)
        );

        ViewData["Title"] = $"Gigs at {Venue.NameLocation}";
        ViewData["Description"] = $"All upcoming gigs at {Venue.Name}. Click on a gig to learn more.";
        ViewData["Image"] = Venue.Image;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";

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

public record VenueRecord
(
    string Name,
    string NameLocation,
    string Address,
    string Description,
    string Website,
    string Image,
    string GoogleMapsUrl
);
