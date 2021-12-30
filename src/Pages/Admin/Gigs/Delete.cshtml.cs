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
                ArtistName = gig.Artist.Name,
                VenueName = gig.Venue.Name,
                Date = gig.Date
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

    public DateTime Date { get; set; }
}
