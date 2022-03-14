namespace GigLocal.Pages;

public class UploadModel : PageModel
{
    private readonly GigContext _context;
    private readonly IImageService _imageService;
    private readonly IRecaptchaService _recaptchaService;
    private readonly ISlackService _slackService;
    private readonly ILogger<UploadModel> _logger;

    [BindProperty]
    public GigUploadModel Gig { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public string RecaptchaSiteKey { get; set; }

    public IEnumerable<SelectListItem> Venues { get; set; }

    public UploadModel(
        GigContext context,
        IImageService storageService,
        IRecaptchaService recaptchaService,
        ISlackService slackService,
        ILogger<UploadModel> logger)
    {
        _context = context;
        _imageService = storageService;
        _recaptchaService = recaptchaService;
        _slackService = slackService;
        _logger = logger;
        RecaptchaSiteKey = _recaptchaService.RecaptchaSiteKey;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Venues = await VenueHelper.GetSelectListAsync(_context);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Venues = await VenueHelper.GetSelectListAsync(_context);

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
            Approved = false
        };

        _context.Gigs.Add(newGig);
        await _context.SaveChangesAsync();

        try
        {
            await _slackService.PostGigUploadedMessageAsync(newGig.ID);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error posting to Slack");
        }

        StatusMessage = $"Gig '{Gig.ArtistName} on {Gig.Date} at {foundVenue.Name}' has been created and is awaiting approval.";
        return RedirectToPage("/Upload");
    }
}

public class GigUploadModel : GigCreateModel
{
    public string RecaptchaToken { get; set; }
}
