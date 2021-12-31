namespace GigLocal.Pages.Admin.Gigs;

public class CreateWithArtistModel : PageModel
{
    private readonly GigContext _context;
    private readonly IArtistService _artistService;

    [BindProperty]
    public GigVenueModel Gig { get; set; }
    [BindProperty]
    public ArtistCreateModel Artist { get; set; }
    public IEnumerable<SelectListItem> Venues { get; set; }

    public CreateWithArtistModel(
        GigContext context,
        IArtistService storageService)
    {
        _context = context;
        _artistService = storageService;
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

        try
        {
            Artist newArtist = await _artistService.CreateAsync(Artist);
            var newGig = new Gig
            {
                ArtistID = newArtist.ID,
                VenueID = foundVenue.ID,
                Date = Gig.Date
            };

            _context.Gigs.Add(newGig);
            await _context.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
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

public class GigVenueModel
{
    [Required]
    [Display(Name = "Venue")]
    public string VenueID { get; set; }

    [Required]
    [FutureDate(ErrorMessage = "The date must be in the future.")]
    [Display(Name = "Date and time")]
    public DateTime Date { get; set; }
}
