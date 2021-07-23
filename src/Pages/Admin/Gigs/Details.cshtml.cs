using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GigLocal.Data;
using GigLocal.Models;
using System.ComponentModel.DataAnnotations;

namespace GigLocal.Pages.Admin.Gigs
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

            [Display(Name = "Artist")]
            public string ArtistName { get; set; }

            [Display(Name = "Venue")]
            public string VenueName { get; set; }

            public DateTime Date { get; set; }

            [Display(Name = "Ticket price")]
            public Decimal TicketPrice { get; set; }

            [Display(Name = "Ticket website")]
            public string TicketWebsite { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
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
