namespace GigLocal.Pages.Admin.Venues;

public class DetailsModel : BasePageModel
{
    private readonly GigContext _context;

    public DetailsModel(GigContext context, MetaTagService metaTagService) : base(metaTagService)
    {
        _context = context;
    }

    public VenueDetailsModel Venue { get; set; }

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

        Venue = new VenueDetailsModel
        {
            ID = venue.ID,
            Name = venue.Name,
            Description = venue.Description,
            Address = VenueHelper.GetFormattedAddress(venue.Address, venue.Suburb, venue.State, venue.Postcode),
            Website = venue.Website,
            ImageUrl = venue.ImageUrl
        };

        ViewData["Title"] = "Details Venue";
        ViewData["Description"] = "See details of a venue on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";

        return Page();
    }
}

public class VenueDetailsModel : VenueReadModel
{
    public int ID { get; set; }
}
