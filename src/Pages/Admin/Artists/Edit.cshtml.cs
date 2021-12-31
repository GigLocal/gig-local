namespace GigLocal.Pages.Admin.Artists;

public class EditModel : PageModel
{
    private readonly GigContext _context;
    private readonly IArtistService _artistService;

    public EditModel(GigContext context, IArtistService storageService)
    {
        _context = context;
        _artistService = storageService;
    }

    [BindProperty]
    public ArtistCreateModel Artist { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var artist = await _context.Artists
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(m => m.ID == id);

        if (artist == null)
        {
            return NotFound();
        }

        Artist = new ArtistCreateModel
        {
            Name = artist.Name,
            Description = artist.Description,
            Website = artist.Website
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _artistService.UpdateAsync(id, Artist);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("./Index");
    }
}
