using GigLocal.Data;
using GigLocal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GigLocal.Pages.Venues
{
    public class CreateModel : PageModel
    {
        private readonly GigContext _context;

        public CreateModel(GigContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public VenueCreateModel Venue { get; set; }

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
                Website = Venue.Website
            };

            _context.Venues.Add(newVenue);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
