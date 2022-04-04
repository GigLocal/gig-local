namespace GigLocal.Pages.Admin.Venues;

public class EditModel : PageModel
{
    private readonly GigContext _context;

    public EditModel(GigContext context)
    {
        _context = context;
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
            Website = venue.Website
        };

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

        venueToUpdate.Name = Venue.Name;
        venueToUpdate.Description = Venue.Description;
        venueToUpdate.Address = Venue.Address;
        venueToUpdate.Website = Venue.Website;
        venueToUpdate.Suburb = Venue.Suburb;
        venueToUpdate.State = Venue.State;
        venueToUpdate.Postcode = Venue.Postcode;

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
