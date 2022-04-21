namespace GigLocal.Pages.Admin.Gigs;

public class DeleteModel : BasePageModel
{
    private readonly GigContext _context;
    private IImageService _imageService;

    public DeleteModel(GigContext context, IImageService imageService, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
        _imageService = imageService;
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

        Gig = new GigReadModel
        {
            ArtistName = gig.ArtistName,
            VenueName = gig.Venue.Name,
            Date = gig.Date,
            Description = gig.Description,
            EventUrl = gig.EventUrl,
            ImageUrl = gig.ImageUrl
        };

        ViewData["Title"] = "Delete Gig";
        ViewData["Description"] = "Delete a gig on Gig Local admin.";
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

        _context.Gigs.Remove(gig);
        await _context.SaveChangesAsync();
        if (gig.ImageUrl != null)
        {
            await _imageService.DeleteImageAsync(gig.ImageUrl);
        }
        return RedirectToPage("./Index");
    }
}

public class GigReadModel
{
    [Display(Name = "Artist")]
    public string ArtistName { get; set; }

    [Display(Name = "Venue")]
    public string VenueName { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
    public DateTime Date { get; set; }

    public string Description { get; set; }

    [Display(Name = "Event URL")]
    public string EventUrl { get; set; }

    [Display(Name = "Image")]
    public string ImageUrl { get; set; }

    public bool Approved { get; set; }
}
