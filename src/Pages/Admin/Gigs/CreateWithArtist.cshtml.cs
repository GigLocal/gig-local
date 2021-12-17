namespace GigLocal.Pages.Admin.Gigs;

public class CreateWithArtistModel : PageModel
{
    private readonly GigContext _context;
    private readonly IStorageService _storageService;
    private readonly ILogger<CreateModel> _logger;
    public IEnumerable<SelectListItem> Venues { get; set; }

    [BindProperty]
    public GigCreateWithArtistModel Gig { get; set; }

    public class GigCreateWithArtistModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Artist name")]
        public string ArtistName { get; set; }

        [Required]
        [Display(Name = "Artist description")]
        public string ArtistDescription { get; set; }

        [Display(Name = "Artist genre")]
        public string ArtistGenre { get; set; }

        [Display(Name = "Artist website")]
        public string ArtistWebsite { get; set; }

        [Display(Name = "Artist image")]
        public IFormFile FormFile { get; set; }

        [Required]
        [Display(Name = "Venue")]
        public string VenueID { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "The date must be in the future.")]
        [Display(Name = "Date and time")]
        public DateTime Date { get; set; }
    }

    public CreateWithArtistModel(
        GigContext context,
        IStorageService storageService,
        ILogger<CreateModel> logger)
    {
        _context = context;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync();
            return Page();
        }

        var foundVenue = await _context.Venues
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(v => v.ID == int.Parse(Gig.VenueID));
        if (foundVenue == null)
        {
            _logger.LogWarning("Venue {Venue} not found", Gig.VenueID);
            await PopulateSelectListsAsync();
            return Page();
        }

        try
        {
            var newArtist = new Artist
            {
                Name = Gig.ArtistName,
                Description = Gig.ArtistDescription,
                Genre = Gig.ArtistGenre,
                Website = Gig.ArtistWebsite
            };
            _context.Artists.Add(newArtist);
            await _context.SaveChangesAsync();

            if (Gig.FormFile?.Length > 0)
            {
                using var formFileStream = Gig.FormFile.OpenReadStream();

                var imageUrl = await _storageService.UploadArtistImageAsync(newArtist.ID, formFileStream);

                newArtist.ImageUrl = imageUrl;
                await _context.SaveChangesAsync();
            }
            var newGig = new Gig
            {
                ArtistID = newArtist.ID,
                VenueID = foundVenue.ID,
                Date = Gig.Date
            };
            _context.Gigs.Add(newGig);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex.Message);
        }

        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task PopulateSelectListsAsync()
    {
        Venues = (await _context.Venues.Select(v => new {Name = v.Name, ID = v.ID})
                                        .OrderBy(v => v.Name)
                                        .ToListAsync())
                                        .Select(v => new SelectListItem(v.Name, v.ID.ToString()));
    }
}
