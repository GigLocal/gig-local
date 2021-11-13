namespace GigLocal.Pages.Admin.Venues;

public class DetailsModel : PageModel
{
    private readonly GigContext _context;

    public DetailsModel(GigContext context)
    {
        _context = context;
    }

    public Venue Venue { get; set; }

    public class VenueDetailsModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Website { get; set; }
    }

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

        Venue = new Venue
        {
            ID = venue.ID,
            Name = venue.Name,
            Description = venue.Description,
            Address = venue.Address,
            Website = venue.Website
        };

        return Page();
    }
}
