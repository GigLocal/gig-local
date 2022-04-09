namespace GigLocal.Pages.Admin.Venues;

public class DeleteModel : PageModel
{
    private readonly GigContext _context;
    private readonly IImageService _imageService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(GigContext context,
                        IImageService imageService,
                        ILogger<DeleteModel> logger)
    {
        _context = context;
        _imageService = imageService;
        _logger = logger;
    }

    public string ErrorMessage { get; set; }

    public VenueReadModel Venue { get; set; }

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
            Venue = new VenueReadModel
            {
                Name = venue.Name,
                Description = venue.Description,
                Address = VenueHelper.GetFormattedAddress(venue.Address, venue.Suburb, venue.State, venue.Postcode),
                Website = venue.Website,
                ImageUrl = venue.ImageUrl
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
            if (venue.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(venue.ImageUrl);
            }
            return RedirectToPage("./Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, ErrorMessage);

            return RedirectToAction("./Delete", new { id, saveChangesError = true });
        }
    }
}

public class VenueReadModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Address { get; set; }

    public string Website { get; set; }

    public string ImageUrl { get; set; }
}
