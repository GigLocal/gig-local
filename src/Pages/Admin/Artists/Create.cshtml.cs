namespace GigLocal.Pages.Admin.Artists;

public class CreateModel : PageModel
{
    private readonly IArtistService _artistService;

    [BindProperty]
    public ArtistCreateModel Artist { get; set; }

    public CreateModel(IArtistService storageService)
    {
        _artistService = storageService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _artistService.CreateAsync(Artist);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("./Index");
    }
}
