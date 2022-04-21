namespace GigLocal.Pages.Admin.Venues;

public class IndexModel : BasePageModel
{
    private readonly GigContext _context;

    public IndexModel(GigContext context, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
    }

    public string CurrentFilter { get; set; }

    public PaginatedList<VenueIndexModel> Venues { get; set; }

    public async Task OnGetAsync(string currentFilter, string searchString, int? pageIndex)
    {
        if (searchString != null)
        {
            pageIndex = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        CurrentFilter = searchString;

        IQueryable<VenueIndexModel> VenuesIQ = _context.Venues.Select(a => new VenueIndexModel{
            ID = a.ID,
            Name = a.Name,
            Description = a.Description,
            Address = VenueHelper.GetFormattedAddress(a.Address, a.Suburb, a.State, a.Postcode),
            Website = a.Website
        });

        if (!string.IsNullOrEmpty(searchString))
        {
            VenuesIQ = VenuesIQ.Where(s => s.Name.Contains(searchString)).OrderByDescending(v => v.Name);
        }

        Venues = await PaginatedList<VenueIndexModel>.CreateAsync(VenuesIQ.AsNoTracking(), pageIndex ?? 1, 10);

        ViewData["Title"] = "Venues";
        ViewData["Description"] = "See venues on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";
    }
}

public class VenueIndexModel
{
    public int ID { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Address { get; set; }

    public string Website { get; set; }
}
