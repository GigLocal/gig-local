namespace GigLocal.Pages.Admin.Gigs;

public class EditModel : PageModel
{
    private readonly GigContext _context;
    private ILogger<EditModel> _logger;
    public IEnumerable<SelectListItem> Artists { get; set; }
    public IEnumerable<SelectListItem> Venues { get; set; }

    [BindProperty]
    public GigEditModel Gig { get; set; }

    public class GigEditModel
    {
        [Required]
        [Display(Name = "Artist")]
        public string ArtistID { get; set; }

        [Required]
        [Display(Name = "Venue")]
        public string VenueID { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "The date must be in the future.")]
        [Display(Name = "Date and time")]
        public DateTime Date { get; set; }

        [Display(Name = "Ticket price")]
        public Decimal TicketPrice { get; set; }

        [Display(Name = "Ticket website")]
        public string TicketWebsite { get; set; }
    }

    public EditModel(GigContext context, ILogger<EditModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var gig = await _context.Gigs.AsNoTracking()
                                        .FirstOrDefaultAsync(g => g.ID == id);

        if (gig == null)
        {
            return NotFound();
        }

        Gig = new GigEditModel
        {
            ArtistID = gig.ArtistID.ToString(),
            VenueID = gig.VenueID.ToString(),
            Date = gig.Date,
            TicketPrice = gig.TicketPrice,
            TicketWebsite = gig.TicketWebsite
        };

        await PopulateSelectListsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync();
            return Page();
        }

        var gigToUpdate = await _context.Gigs.FindAsync(id);

        if (gigToUpdate == null)
        {
            return NotFound();
        }

        var foundArtist = await _context.Artists
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(a => a.ID == int.Parse(Gig.ArtistID));
        if (foundArtist == null)
        {
            _logger.LogWarning("Artist {Artist} not found", Gig.ArtistID);
            await PopulateSelectListsAsync();
            return Page();
        }
        var foundVenue = await _context.Venues
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(v => v.ID == int.Parse(Gig.VenueID));
        if (foundVenue == null)
        {
            _logger.LogWarning("Venue {Venue} not found", Gig.VenueID);
            await PopulateSelectListsAsync();
            return Page();
        }

        try
        {
            gigToUpdate.ArtistID = foundArtist.ID;
            gigToUpdate.VenueID = foundVenue.ID;
            gigToUpdate.Date = Gig.Date;
            gigToUpdate.TicketPrice = Gig.TicketPrice;
            gigToUpdate.TicketWebsite = Gig.TicketWebsite;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex.Message);
        }

        await PopulateSelectListsAsync();
        return Page();
    }

    public async Task PopulateSelectListsAsync()
    {
        Artists = (await _context.Artists.Select(a => new {Name = a.Name, ID = a.ID})
                                            .OrderBy(a => a.Name)
                                            .ToListAsync())
                                            .Select(a => new SelectListItem(a.Name, a.ID.ToString()));

        Venues = (await _context.Venues.Select(v => new {Name = v.Name, ID = v.ID})
                                        .OrderBy(v => v.Name)
                                        .ToListAsync())
                                        .Select(v => new SelectListItem(v.Name, v.ID.ToString()));
    }
}
