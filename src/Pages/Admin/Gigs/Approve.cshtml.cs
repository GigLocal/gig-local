namespace GigLocal.Pages.Admin.Gigs;

public class ApproveModel : BasePageModel
{
    private readonly GigContext _context;

    public ApproveModel(GigContext context, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
    }

    public string ErrorMessage { get; set; }

    public GigReadModel Gig { get; set; }

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

        if (gig.Approved)
        {
            return RedirectToPage("./Index");
        }

        Gig = new GigReadModel
        {
            ArtistName = gig.ArtistName,
            VenueName = gig.Venue.Name,
            Date = gig.Date,
            Description = gig.Description,
            EventUrl = gig.EventUrl,
            ImageUrl = gig.ImageUrl
        };

        ViewData["Title"] = "Approve Gig";
        ViewData["Description"] = "Approve a gig on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (!ModelState.IsValid)
            return Page();

        if (id == null)
        {
            return NotFound();
        }

        var gig = await _context.Gigs.FindAsync(id);

        if (gig == null)
        {
            return NotFound();
        }

        gig.Approved = true;
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
