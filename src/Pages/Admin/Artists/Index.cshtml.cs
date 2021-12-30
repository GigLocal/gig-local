namespace GigLocal.Pages.Admin.Artists;

public class IndexModel : PageModel
{
    private readonly GigContext _context;

    public IndexModel(GigContext context)
    {
        _context = context;
    }

    public string CurrentFilter { get; set; }

    public PaginatedList<ArtistIndexModel> Artists { get; set; }

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

        IQueryable<ArtistIndexModel> ArtistsIQ = _context.Artists.Select(a => new ArtistIndexModel{
            ID = a.ID,
            Name = a.Name,
            Description = a.Description,
            Website = a.Website
        });

        if (!string.IsNullOrEmpty(searchString))
        {
            ArtistsIQ = ArtistsIQ.Where(s => s.Name.Contains(searchString)).OrderByDescending(a => a.Name);
        }

        Artists = await PaginatedList<ArtistIndexModel>.CreateAsync(ArtistsIQ.AsNoTracking(), pageIndex ?? 1, 10);
    }
}

public class ArtistIndexModel
{
    public int ID { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Website { get; set; }
}
