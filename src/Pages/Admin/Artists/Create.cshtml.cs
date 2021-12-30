namespace GigLocal.Pages.Admin.Artists;

public class CreateModel : PageModel
{
    private readonly GigContext _context;
    private readonly IStorageService _storageService;

    [BindProperty]
    public ArtistCreateModel Artist { get; set; }

    public CreateModel(GigContext context, IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
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

        if (_context.Artists.Any(a => a.Name.ToLower() == Artist.Name.ToLower()))
        {
            ModelState.AddModelError(string.Empty, $"Artist with name '{Artist.Name}' already exists.");
            return Page();
        }

        var newArtist = new Artist
        {
            Name = Artist.Name,
            Description = Artist.Description,
            Website = Artist.Website
        };

        _context.Artists.Add(newArtist);
        await _context.SaveChangesAsync();

        if (Artist.FormFile?.Length > 0)
        {
            using var formFileStream = Artist.FormFile.OpenReadStream();

            var imageUrl = await _storageService.UploadArtistImageAsync(newArtist.ID, formFileStream);

            newArtist.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}

public class ArtistCreateModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; }

    public string Website { get; set; }

    [Display(Name = "Image")]
    public IFormFile FormFile { get; set; }
}
