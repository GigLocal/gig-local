namespace GigLocal.Pages.Admin.Gigs;

public class CreateModel : BasePageModel
{
    private readonly GigContext _context;
    private readonly IImageService _imageService;

    [BindProperty]
    public GigCreateModel Gig { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public IEnumerable<SelectListItem> Venues { get; set; }

    public CreateModel(
        GigContext context,
        IImageService storageService,
        IRecaptchaService recaptchaService,
        MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
        _imageService = storageService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Venues = await VenueHelper.GetSelectListAsync(_context);

        ViewData["Title"] = "Create Gig";
        ViewData["Description"] = "Create a gig on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Venues = await VenueHelper.GetSelectListAsync(_context);

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

        var newGig = new Gig
        {
            VenueID = foundVenue.ID,
            StartDate = Gig.StartDate,
            EndDate = Gig.EndDate,
            ArtistName = Gig.ArtistName,
            Description = Gig.Description,
            EventUrl = Gig.EventUrl,
            ImageUrl = imageUrl,
            Approved = true
        };

        _context.Gigs.Add(newGig);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}

public class GigCreateModel : IValidatableObject
{
    [Required]
    [Display(Name = "Venue")]
    public string VenueID { get; set; }

    [Required]
    [FutureDate(ErrorMessage = "Start date must be in the future.")]
    [Display(Name = "Start date and time")]
    public DateTime StartDate { get; set; }

    [Required]
    [FutureDate(ErrorMessage = "End date must be in the future.")]
    [Display(Name = "End date and time")]
    public DateTime? EndDate { get; set; }

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
       if (EndDate <= StartDate)
       {
           yield return new ValidationResult(
               errorMessage: "End date/time must be after Start date/time",
               memberNames: new[] { "EndDate" }
          );
       }
    }
}
