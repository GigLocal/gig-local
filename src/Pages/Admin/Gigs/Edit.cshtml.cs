namespace GigLocal.Pages.Admin.Gigs;

public class EditModel : PageModel
{
    private readonly GigContext _context;
    private IImageService _imageService;
    public IEnumerable<SelectListItem> Venues { get; set; }

    [BindProperty]
    public GigCreateModel Gig { get; set; }

    public EditModel(GigContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var gig = await _context.Gigs.AsNoTracking()
                                        .Include(g => g.Venue)
                                        .FirstOrDefaultAsync(g => g.ID == id);

        if (gig == null)
        {
            return NotFound();
        }

        Gig = new GigCreateModel
        {
            VenueID = gig.VenueID.ToString(),
            Date = gig.Date,
            ArtistName = gig.ArtistName,
            Description = gig.Description,
            EventUrl = gig.EventUrl
        };

        await PopulateSelectListsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await PopulateSelectListsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var gigToUpdate = await _context.Gigs.FindAsync(id);

        if (gigToUpdate == null)
        {
            return NotFound();
        }

        var foundVenue = await _context.Venues
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(v => v.ID == int.Parse(Gig.VenueID));
        if (foundVenue == null)
        {
            return NotFound();
        }

        if (gigToUpdate.ImageUrl != null)
        {
            await _imageService.DeleteImageAsync(gigToUpdate.ImageUrl);
        }

        using var imageStream = Gig.FormFile.OpenReadStream();
        var imageUrl = await _imageService.UploadImageAsync(imageStream);

        gigToUpdate.VenueID = foundVenue.ID;
        gigToUpdate.Date = Gig.Date;
        gigToUpdate.ArtistName = Gig.ArtistName;
        gigToUpdate.Description = Gig.Description;
        gigToUpdate.EventUrl = Gig.EventUrl;
        gigToUpdate.ImageUrl = imageUrl;

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
