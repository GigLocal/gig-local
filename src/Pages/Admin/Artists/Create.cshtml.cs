namespace GigLocal.Pages.Admin.Artists;

public class CreateModel : PageModel
{
    private readonly GigContext _context;
    private readonly IStorageService _storageService;

    [BindProperty]
    public ArtistCreateModel Artist { get; set; }

    public class ArtistCreateModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Website { get; set; }

        [Display(Name = "Image")]
        public IFormFile FormFile { get; set; }
    }

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

        var newArtist = new Artist
        {
            Name = Artist.Name,
            Description = Artist.Description,
            Genre = Artist.Genre,
            Website = Artist.Website,
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
