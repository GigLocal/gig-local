namespace GigLocal.Pages.Admin.Gigs;

public class DuplicateModel : BasePageModel
{
    private readonly GigContext _context;
    private readonly IImageService _imageService;

    [BindProperty]
    public GigDuplicateModel Gig { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public IEnumerable<SelectListItem> Venues { get; set; }

    public DuplicateModel(
        GigContext context,
        IImageService storageService,
        IRecaptchaService recaptchaService,
        MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
        _imageService = storageService;
    }

    public async Task<IActionResult> OnGetAsync(int? templateId)
    {
        Venues = await VenueHelper.GetSelectListAsync(_context);

        if (templateId != null)
        {
            Gig gig = await _context.Gigs
                .AsNoTracking()
                .Include(g => g.Venue)
                .FirstOrDefaultAsync(m => m.ID == templateId);

            if (gig == null)
            {
                return NotFound();
            }

            Gig = new GigDuplicateModel
            {
                ArtistName = gig.ArtistName,
                VenueID = gig.VenueID.ToString(),
                Description = gig.Description,
                ImageUrl = gig.ImageUrl,
                StartDate = null,
                EndDate = null
            };
        }

        ViewData["Title"] = "Duplicate Gig";
        ViewData["Description"] = "Duplicate a gig on Gig Local admin.";
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

        string imageUrl = await _imageService.CopyImageAsync(Gig.ImageUrl);

        var newGig = new Gig
        {
            VenueID = foundVenue.ID,
            StartDate = (DateTime)Gig.StartDate,
            EndDate = (DateTime)Gig.EndDate,
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

public class GigDuplicateModel : IValidatableObject
{
    [Required]
    [Display(Name = "Venue")]
    public string VenueID { get; set; }

    [Required]
    [FutureDate(ErrorMessage = "Start date must be in the future.")]
    [Display(Name = "Start date and time")]
    public DateTime? StartDate { get; set; }

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
    public string ImageUrl { get; set; }

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
