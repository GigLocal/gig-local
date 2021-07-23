using GigLocal.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GigLocal.Pages.Admin.Gigs
{
    public class IndexModel : PageModel
    {
        private readonly GigContext _context;

        public IndexModel(GigContext context)
        {
            _context = context;
        }

        public string CurrentFilter { get; set; }

        public PaginatedList<GigIndexModel> Gigs { get; set; }

        public class GigIndexModel
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

        public async Task OnGetAsync(string currentFilter, string searchString, int? pageIndex)
        {
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<GigIndexModel> GigsIQ = _context.Gigs
                .Include(g => g.Artist)
                .Include(g => g.Venue)
                .Select(a => new GigIndexModel{
                    ID = a.ID,
                    ArtistName = a.Artist.Name,
                    VenueName = a.Venue.Name,
                    Date = a.Date,
                    TicketPrice = a.TicketPrice,
                    TicketWebsite = a.TicketWebsite
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                GigsIQ = GigsIQ.Where(s =>
                    s.ArtistName.Contains(searchString)|| s.VenueName.Contains(searchString)).OrderByDescending(g => g.Date);
            }

            Gigs = await PaginatedList<GigIndexModel>.CreateAsync(GigsIQ.AsNoTracking(), pageIndex ?? 1, 10);
        }
    }
}
