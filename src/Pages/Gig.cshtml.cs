using Schema.NET;

namespace GigLocal.Pages;

public class GigModel : BasePageModel
{
    private readonly GigContext _context;

    public GigDetailRecord Gig { get; set; }

    public GigModel(GigContext context, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(string gigName, int? gigId)
    {
        if (gigId is null)
        {
            return NotFound();
        }

        var gig = await _context.Gigs
                                  .AsNoTracking()
                                  .Include(g => g.Venue)
                                  .FirstOrDefaultAsync(g => g.ID == gigId);
        if (gig is null)
        {
            return NotFound();
        }

        Gig = new GigDetailRecord(
            GigHelper.GetDateTime(gig.StartDate, gig.EndDate),
            gig.StartDate,
            (DateTime)gig.EndDate,
            gig.ArtistName,
            gig.Description,
            gig.EventUrl,
            gig.ImageUrl,
            gig.Venue.Name,
            gig.Venue.Address,
            gig.Venue.Suburb,
            gig.Venue.State,
            gig.Venue.Postcode,
            gig.Venue.Website,
            VenueHelper.GetGoogleMapsUrl(gig.Venue.Name, gig.Venue.Address, gig.Venue.Suburb, gig.Venue.State, gig.Venue.Postcode)
        );

        var venueTimeZone = TimeZoneInfo.FindSystemTimeZoneById(gig.Venue.TimeZone);

        var googleEvent = new Event {
            Name = Gig.ArtistName,
            StartDate = new DateTimeOffset(Gig.StartDate, venueTimeZone.GetUtcOffset(Gig.StartDate)),
            EndDate = new DateTimeOffset(Gig.EndDate, venueTimeZone.GetUtcOffset(Gig.EndDate)),
            EventStatus = EventStatusType.EventScheduled,
            Location = new Place {
                Name = Gig.VenueName,
                Address = new PostalAddress {
                    StreetAddress = Gig.VenueAddress,
                    AddressLocality = Gig.VenueSuburb,
                    AddressRegion = Gig.VenueState,
                    PostalCode = Gig.VenuePostcode.ToString(),
                    AddressCountry = "Australia"
                }
            },
            Image = new [] {new Uri(Gig.Image)},
            Description = Gig.Description,
            Performer = new PerformingGroup {
                Name = Gig.ArtistName
            },
            Organizer = new Organization {
                Name = Gig.VenueName,
                Url = new Uri(Gig.VenueWebsite)
            }
        };

        ViewData["Title"] = $"{Gig.ArtistName} at {Gig.VenueName}, {Gig.StartDate:ddd d MMM yyyy}";
        ViewData["Description"] = Gig.Description;
        ViewData["Image"] = Gig.Image;
        ViewData["Url"] = HttpContext.Request.GetDisplayUrl();
        ViewData["GoogleEventJson"] = googleEvent.ToHtmlEscapedString();

        return Page();
    }
}

public record GigDetailRecord(
    string DateTime,
    DateTime StartDate,
    DateTime EndDate,
    string ArtistName,
    string Description,
    string EventUrl,
    string Image,
    string VenueName,
    string VenueAddress,
    string VenueSuburb,
    string VenueState,
    int VenuePostcode,
    string VenueWebsite,
    string GoogleMapsUrl
);
