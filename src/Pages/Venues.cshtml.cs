namespace GigLocal.Pages;

public class VenuesModel : BasePageModel
{
    private readonly GigContext _context;

    public IEnumerable<VenueListRecord> Venues { get; set; }

    public VenuesModel(GigContext context, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        var venuesQuery = _context.Venues
            .AsNoTracking()
            .OrderBy(g => g.Name);

        var venues = await venuesQuery.ToArrayAsync();

        Venues = venuesQuery.Select(v => new VenueListRecord(
            v.Name,
            v.Description,
            VenueHelper.GetFormattedAddress(v.Address, v.Suburb, v.State, v.Postcode),
            VenueHelper.GetUrlFriendlyName(v.ID, v.Name, v.Suburb, v.State),
            v.ImageUrl
        ));

        ViewData["Title"] = "Venues";
        ViewData["Description"] = "All venues on Gig Local. Click on a venue to see upcoming gigs or to learn more.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = MetaTagService.VenuesUrl;
    }
}

public record VenueListRecord
(
    string Name,
    string Description,
    string Address,
    string Url,
    string ImageUrl
);
