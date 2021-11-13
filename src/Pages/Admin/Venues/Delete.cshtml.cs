namespace GigLocal.Pages.Admin.Venues;

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

    public VenueDeleteModel Venue { get; set; }

    public class VenueDeleteModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Website { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
    {
        if (id == null)
        {
            return NotFound();
        }

        Venue venue = await _context.Venues
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(m => m.ID == id);

        if (venue == null)
        {
            return NotFound();
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ErrorMessage = String.Format("Delete {ID} failed. Try again", id);
        }
        else
        {
            Venue = new VenueDeleteModel
            {
                Name = venue.Name,
                Description = venue.Description,
                Address = venue.Address,
                Website = venue.Website
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
            return NotFound(new { message = "hello"});
        }

        var venue = await _context.Venues.FindAsync(id);

        if (venue == null)
        {
            return NotFound();
        }

        try
        {
            _context.Venues.Remove(venue);
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
