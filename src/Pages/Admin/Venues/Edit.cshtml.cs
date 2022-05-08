namespace GigLocal.Pages.Admin.Venues;

public class EditModel : BasePageModel
{
    private readonly GigContext _context;

    private readonly IImageService _imageService;

    public EditModel(GigContext context, IImageService imageService, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
        _imageService = imageService;
    }

    [BindProperty]
    public VenueCreateModel Venue { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Venue venue = await _context.Venues
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(m => m.ID == id);

        if (venue == null)
        {
            return NotFound();
        }

        Venue = new VenueCreateModel
        {
            Name = venue.Name,
            Description = venue.Description,
            Address = venue.Address,
            Suburb = venue.Suburb,
            State = venue.State,
            Postcode = venue.Postcode,
            Website = venue.Website,
            TimeZone = venue.TimeZone
        };

        ViewData["Title"] = "Edit Venue";
        ViewData["Description"] = "Edit a venue on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (!ModelState.IsValid)
            return Page();

        if (id == null)
        {
            return NotFound();
        }

        var venueToUpdate = await _context.Venues.FindAsync(id);

        if (venueToUpdate == null)
        {
            return NotFound();
        }

        if (venueToUpdate.ImageUrl != null)
        {
            await _imageService.DeleteImageAsync(venueToUpdate.ImageUrl);
        }

        using var imageStream = Venue.FormFile.OpenReadStream();
        var imageUrl = await _imageService.UploadImageAsync(imageStream);

        venueToUpdate.Name = Venue.Name;
        venueToUpdate.Description = Venue.Description;
        venueToUpdate.Address = Venue.Address;
        venueToUpdate.Website = Venue.Website;
        venueToUpdate.Suburb = Venue.Suburb;
        venueToUpdate.State = Venue.State;
        venueToUpdate.Postcode = Venue.Postcode;
        venueToUpdate.ImageUrl = imageUrl;
        venueToUpdate.TimeZone = Venue.TimeZone;

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
