using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GigLocal.Data;
using GigLocal.Models;

namespace GigLocal.Pages.Gigs
{
    public class DetailsModel : PageModel
    {
        private readonly GigContext _context;

        public DetailsModel(GigContext context)
        {
            _context = context;
        }

        public GigDetialsModel Gig { get; set; }

        public class GigDetialsModel
        {
            public int ID { get; set; }

            public string ArtistName { get; set; }

            public string VenueName { get; set; }

            public DateTime Date { get; set; }

            public Decimal TicketPrice { get; set; }

            public string TicketWebsite { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!ModelState.IsValid)
                return Page();

            if (id == null)
            {
                return NotFound();
            }

            Gig gig = await _context.Gigs
                .AsNoTracking()
                .Include(g => g.Artist)
                .Include(g => g.Venue)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (gig == null)
            {
                return NotFound();
            }

            Gig = new GigDetialsModel
            {
                ID = gig.ID,
                ArtistName = gig.Artist.Name,
                VenueName = gig.Venue.Name,
                Date = gig.Date,
                TicketPrice = gig.TicketPrice,
                TicketWebsite = gig.TicketWebsite
            };

            return Page();
        }
    }
}
