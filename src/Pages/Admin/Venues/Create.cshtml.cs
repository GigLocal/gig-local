namespace GigLocal.Pages.Admin.Venues;

public class CreateModel : BasePageModel
{
    private readonly GigContext _context;
    private readonly IImageService _imageService;

    [BindProperty]
    public VenueCreateModel Venue { get; set; }

    public CreateModel(GigContext context, IImageService storageService, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
        _imageService = storageService;
    }

    public void OnGet()
    {
        ViewData["Title"] = "Create Venue";
        ViewData["Description"] = "Create a venue on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        using var imageStream = Venue.FormFile.OpenReadStream();
        var imageUrl = await _imageService.UploadImageAsync(imageStream);

        var newVenue = new Venue
        {
            Name = Venue.Name,
            Description = Venue.Description,
            Address = Venue.Address,
            Website = Venue.Website,
            Suburb = Venue.Suburb,
            State = Venue.State,
            Postcode = Venue.Postcode,
            ImageUrl = imageUrl
        };

        _context.Venues.Add(newVenue);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}

public class VenueCreateModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [MaxLength(300)]
    public string Description { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string Website { get; set; }

    [Required]
    public string Suburb { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public int Postcode { get; set; }

    [Required]
    [Display(Name = "Image")]
    public IFormFile FormFile { get; set; }
}
