namespace GigLocal.Pages.Admin.Gigs;

public class IndexModel : BasePageModel
{
    private readonly GigContext _context;

    public IndexModel(GigContext context, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
    }

    public string CurrentFilter { get; set; }

    public bool PendingFilter { get; set; }

    public PaginatedList<GigIndexModel> Gigs { get; set; }

    public async Task OnGetAsync(string currentFilter, string searchString, int? pageIndex, bool pending = false)
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
        PendingFilter = pending;

        IQueryable<GigIndexModel> GigsIQ = _context.Gigs
            .Include(g => g.Venue)
            .Select(a => new GigIndexModel{
                ID = a.ID,
                ArtistName = a.ArtistName,
                VenueName = a.Venue.Name,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Approved = a.Approved
            });

        if (!string.IsNullOrEmpty(searchString))
        {
            GigsIQ = GigsIQ.Where(s =>
                s.ArtistName.Contains(searchString)|| s.VenueName.Contains(searchString));
        }

        if (pending)
        {
            GigsIQ = GigsIQ.Where(g => !g.Approved);
        }

        GigsIQ = GigsIQ.OrderByDescending(g => g.StartDate);

        Gigs = await PaginatedList<GigIndexModel>.CreateAsync(GigsIQ.AsNoTracking(), pageIndex ?? 1, 10);

        ViewData["Title"] = "Gigs";
        ViewData["Description"] = "See gigs on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";
    }
}

public class GigIndexModel
{
    public int ID { get; set; }

    [Display(Name = "Artist")]
    public string ArtistName { get; set; }

    [Display(Name = "Venue")]
    public string VenueName { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
    public DateTime StartDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
    public DateTime EndDate { get; set; }

    public bool Approved { get; set; }
}
