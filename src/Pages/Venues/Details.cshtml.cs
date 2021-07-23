using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GigLocal.Data;
using GigLocal.Models;

namespace GigLocal.Pages.Venues
{
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
                Name = venue.Name,
                Description = venue.Description,
                Address = venue.Address,
                Website = venue.Website
            };

            return Page();
        }
    }
}
