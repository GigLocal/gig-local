namespace GigLocal.Pages;

public class VenuesModel : PageModel
{
    private readonly GigContext _context;

    private readonly LinkGenerator _linkGenerator;

    public IEnumerable<VenueListRecord> Venues { get; set; }

    public VenuesModel(GigContext context, LinkGenerator linkGenerator)
    {
        _context = context;
        _linkGenerator = linkGenerator;
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
            $"/venues/{v.ID}/{VenueHelper.GetUrlFriendlyName(v.Name, v.Suburb, v.State)}",
            v.ImageUrl
        ));
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
