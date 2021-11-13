namespace GigLocal.Pages.Admin.Artists;

public class EditModel : PageModel
{
    private readonly GigContext _context;
    private readonly IStorageService _storageService;

    public EditModel(GigContext context, IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    [BindProperty]
    public ArtistEditModel Artist { get; set; }

    public class ArtistEditModel
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

        Artist = new ArtistEditModel
        {
            Name = artist.Name,
            Description = artist.Description,
            Genre = artist.Genre,
            Website = artist.Website
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var artistToUpdate = await _context.Artists.FindAsync(id);

        if (artistToUpdate == null)
        {
            return NotFound();
        }

        artistToUpdate.Name = Artist.Name;
        artistToUpdate.Description = Artist.Description;
        artistToUpdate.Genre = Artist.Genre;
        artistToUpdate.Website = Artist.Website;

        if (Artist.FormFile?.Length > 0)
        {
            using var formFileStream = Artist.FormFile.OpenReadStream();
            var imageUrl = await _storageService.UploadArtistImageAsync(artistToUpdate.ID, formFileStream);

            artistToUpdate.ImageUrl = imageUrl;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
