namespace GigLocal.Pages.Admin.Venues;

public class CreateModel : PageModel
{
    private readonly GigContext _context;

    [BindProperty]
    public VenueCreateModel Venue { get; set; }

    public CreateModel(GigContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var newVenue = new Venue
        {
            Name = Venue.Name,
            Description = Venue.Description,
            Address = Venue.Address,
            Website = Venue.Website,
            Suburb = Venue.Suburb,
            State = Venue.State,
            Postcode = Venue.Postcode
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
}
