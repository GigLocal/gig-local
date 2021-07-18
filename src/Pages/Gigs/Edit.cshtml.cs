using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GigLocal.Data;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GigLocal.Pages.Gigs
{
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
            public string ArtistID { get; set; }
            [Required]
            public string VenueID { get; set; }
            [Required]
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

            var gig = await _context.Gigs.FindAsync(id);

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
                return Page();
            }

            var gigToUpdate = await _context.Gigs.FindAsync(id);

            if (gigToUpdate == null)
            {
                return NotFound();
            }

            var foundArtist = await _context.Artists.FindAsync(int.Parse(Gig.ArtistID));
            if (foundArtist != null)
            {
                gigToUpdate.ArtistID = foundArtist.ID;
            }
            else
            {
                _logger.LogWarning("Artist {Artist} not found", Gig.ArtistID);
                return Page();
            }
            var foundVenue = await _context.Venues.FindAsync(int.Parse(Gig.VenueID));
            if (foundVenue != null)
            {
                gigToUpdate.VenueID = foundVenue.ID;
            }
            else
            {
                _logger.LogWarning("Venue {Venue} not found", Gig.VenueID);
                return Page();
            }

            try
            {
                gigToUpdate.Date = Gig.Date;
                gigToUpdate.TicketPrice = Gig.TicketPrice;
                gigToUpdate.TicketWebsite = Gig.TicketWebsite;
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
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
}
