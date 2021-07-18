using GigLocal.Data;
using GigLocal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GigLocal.Pages.Venues
{
    public class EditModel : PageModel
    {
        private readonly GigContext _context;

        public EditModel(GigContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VenueEditModel Venue { get; set; }

        public class VenueEditModel
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Venue venue = await _context.Venues.FirstOrDefaultAsync(m => m.ID == id);

            if (venue == null)
            {
                return NotFound();
            }

            Venue = new VenueEditModel
            {
                Name = venue.Name,
                Description = venue.Description,
                Address = venue.Address,
                Website = venue.Website
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venueToUpdate = await _context.Venues.FindAsync(id);

            if (venueToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Venue>(
                 venueToUpdate,
                 "venue",   // Prefix for form value.
                   v => v.Name, v => v.Description, v => v.Website, v => v.Address))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
