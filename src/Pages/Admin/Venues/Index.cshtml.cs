namespace GigLocal.Pages.Admin.Venues;

public class IndexModel : PageModel
{
    private readonly GigContext _context;

    public IndexModel(GigContext context)
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
            Address = a.Address,
            Website = a.Website
        });

        if (!string.IsNullOrEmpty(searchString))
        {
            VenuesIQ = VenuesIQ.Where(s => s.Name.Contains(searchString)).OrderByDescending(v => v.Name);
        }

        Venues = await PaginatedList<VenueIndexModel>.CreateAsync(VenuesIQ.AsNoTracking(), pageIndex ?? 1, 10);
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
