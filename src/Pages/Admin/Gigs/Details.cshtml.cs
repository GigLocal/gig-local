namespace GigLocal.Pages.Admin.Gigs;

public class DetailsModel : PageModel
{
    private readonly GigContext _context;

    public DetailsModel(GigContext context)
    {
        _context = context;
    }

    public GigDetialsModel Gig { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Gig gig = await _context.Gigs
            .AsNoTracking()
            .Include(g => g.Venue)
            .FirstOrDefaultAsync(m => m.ID == id);

        if (gig == null)
        {
            return NotFound();
        }

        Gig = new GigDetialsModel
        {
            ID = gig.ID,
            ArtistName = gig.ArtistName,
            VenueName = gig.Venue.Name,
            Date = gig.Date,
            Description = gig.Description,
            EventUrl = gig.EventUrl,
            ImageUrl = gig.ImageUrl,
            Approved = gig.Approved
        };

        return Page();
    }
}

public class GigDetialsModel : GigReadModel
{
    public int ID { get; set; }
}
