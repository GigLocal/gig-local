namespace GigLocal.Pages.Admin.Gigs;

public class DeleteModel : PageModel
{
    private readonly GigContext _context;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(GigContext context,
                        ILogger<DeleteModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public string ErrorMessage { get; set; }

    public GigReadModel Gig { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
    {
        if (id == null)
        {
            return NotFound();
        }

        Gig gig = await _context.Gigs
            .AsNoTracking()
            .Include(g => g.Artist)
            .Include(g => g.Venue)
            .FirstOrDefaultAsync(m => m.ID == id);

        if (gig == null)
        {
            return NotFound();
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ErrorMessage = String.Format("Delete {ID} failed. Try again", id);
        }
        else
        {
            Gig = new GigReadModel
            {
                ArtistName = gig.ArtistName ?? gig.Artist.Name,
                VenueName = gig.Venue.Name,
                Date = gig.Date,
                Description = gig.Description ?? gig.Artist.Description,
                EventUrl = gig.EventUrl ?? gig.Venue.Website,
                ImageUrl = gig.ImageUrl ?? gig.Artist.ImageUrl
            };
        }

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

        try
        {
            _context.Gigs.Remove(gig);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, ErrorMessage);

            return RedirectToAction("./Delete", new { id, saveChangesError = true });
        }
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
}
