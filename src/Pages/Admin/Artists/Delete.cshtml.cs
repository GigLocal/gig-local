namespace GigLocal.Pages.Admin.Artists;

public class DeleteModel : PageModel
{
    private readonly GigContext _context;
    private readonly ILogger<DeleteModel> _logger;
    private readonly IStorageService _storageService;

    public DeleteModel(GigContext context,
                        ILogger<DeleteModel> logger,
                        IStorageService storageService)
    {
        _context = context;
        _logger = logger;
        _storageService = storageService;
    }

    public string ErrorMessage { get; set; }

    public ArtistReadModel Artist { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
    {
        if (id == null)
        {
            return NotFound();
        }

        Artist artist = await _context.Artists
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.ID == id);

        if (artist == null)
        {
            return NotFound();
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ErrorMessage = String.Format("Delete {ID} failed. Try again", id);
        }
        else
        {
            Artist = new ArtistReadModel
            {
                Name = artist.Name,
                Description = artist.Description,
                Website = artist.Website,
                ImageUrl = artist.ImageUrl
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

        var artist = await _context.Artists.FindAsync(id);

        if (artist == null)
        {
            return NotFound();
        }

        try
        {
            if (artist.ImageUrl != null)
            {
                await _storageService.DeleteArtistImageAsync(artist.ID);
            }
            _context.Artists.Remove(artist);
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

public class ArtistReadModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Website { get; set; }


    [Display(Name = "Image")]
    public string ImageUrl { get; set; }
}
