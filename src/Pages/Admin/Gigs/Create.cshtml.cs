namespace GigLocal.Pages.Admin.Gigs;

public class CreateModel : PageModel
{
    private readonly GigContext _context;
    private readonly IImageService _imageService;
    private readonly IRecaptchaService _recaptchaService;

    [BindProperty]
    public GigCreateModel Gig { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public string RecaptchaSiteKey { get; set; }

    public IEnumerable<SelectListItem> Venues { get; set; }

    public CreateModel(
        GigContext context,
        IImageService storageService,
        IRecaptchaService recaptchaService)
    {
        _context = context;
        _imageService = storageService;
        _recaptchaService = recaptchaService;
        RecaptchaSiteKey = _recaptchaService.RecaptchaSiteKey;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        var passedRecaptcha = await _recaptchaService.ValidateAsync(Gig.RecaptchaToken);
        if (!passedRecaptcha)
        {
            StatusMessage = "Error: sorry, no robots allowed!";
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var foundVenue = await _context.Venues
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(v => v.ID == int.Parse(Gig.VenueID));
        if (foundVenue == null)
        {
            return NotFound();
        }

        using var imageStream = Gig.FormFile.OpenReadStream();
        var imageUrl = await _imageService.UploadImageAsync(imageStream);

        // To preserve existing data model, can be removed in future
        // once all old data is no longer needed.
        var placeholderArtist = new Artist();
        _context.Artists.Add(placeholderArtist);
        await _context.SaveChangesAsync();

        var newGig = new Gig
        {
            ArtistID = placeholderArtist.ID,
            VenueID = foundVenue.ID,
            Date = Gig.Date,
            ArtistName = Gig.ArtistName,
            Description = Gig.Description,
            EventUrl = Gig.EventUrl,
            ImageUrl = imageUrl,
            Approved = HttpContext.User.Identity.IsAuthenticated
        };

        _context.Gigs.Add(newGig);
        await _context.SaveChangesAsync();

        if (!newGig.Approved)
        {
            // Status message with basic gig detials
            StatusMessage = $"Gig '{Gig.ArtistName} on {Gig.Date} at {foundVenue.Name}' has been created and is awaiting approval.";
            return RedirectToPage("/Admin/Gigs/Create");
        }

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
    [Display(Name = "Artist name")]
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

    public string RecaptchaToken { get; set; }
}
