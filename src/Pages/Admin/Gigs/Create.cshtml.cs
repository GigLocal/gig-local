namespace GigLocal.Pages.Admin.Gigs;

public class CreateWithArtistModel : PageModel
{
    private readonly GigContext _context;
    private readonly IImageService _imageService;

    [BindProperty]
    public GigCreateModel Gig { get; set; }

    public IEnumerable<SelectListItem> Venues { get; set; }

    public CreateWithArtistModel(
        GigContext context,
        IImageService storageService)
    {
        _context = context;
        _imageService = storageService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var foundVenue = await _context.Venues
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(v => v.ID == int.Parse(Gig.VenueID));
        if (foundVenue == null)
        {
            return NotFound($"Venue with ID '{Gig.VenueID}' could not be found.");
        }

        using var imageStream = Gig.FormFile.OpenReadStream();
        var imageUrl = await _imageService.UploadImageAsync(imageStream);

        var placeholderArtist = new Artist();

        // To preserve existing data model, can be removed in future.
        _context.Artists.Add(placeholderArtist);

        var newGig = new Gig
        {
            ArtistID = placeholderArtist.ID,
            VenueID = foundVenue.ID,
            Date = Gig.Date,
            ArtistName = Gig.ArtistName,
            Description = Gig.Description,
            EventUrl = Gig.EventUrl,
            ImageUrl = imageUrl
        };

        _context.Gigs.Add(newGig);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    public async Task PopulateSelectListsAsync()
    {
        Venues = (await _context.Venues.Select(v => new {Name = v.Name, ID = v.ID})
                                        .OrderBy(v => v.Name)
                                        .ToListAsync())
                                        .Select(v => new SelectListItem(v.Name, v.ID.ToString()));
    }
}

public class GigCreateModel
{
    [Required]
    [Display(Name = "Venue")]
    public string VenueID { get; set; }

    [Required]
    [FutureDate(ErrorMessage = "The date must be in the future.")]
    [Display(Name = "Date and time")]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "Artist Name")]
    public string ArtistName { get; set; }

    [Required]
    [MaxLength(300)]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Event URL")]
    public string EventUrl { get; set; }

    [Required]
    [Display(Name = "Image")]
    public IFormFile FormFile { get; set; }
}
