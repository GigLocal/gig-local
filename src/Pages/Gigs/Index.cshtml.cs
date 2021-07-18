using GigLocal.Data;
using GigLocal.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GigLocal.Pages.Gigs
{
    public class IndexModel : PageModel
    {
        private readonly GigContext _context;
        private readonly IConfiguration Configuration;

        public IndexModel(GigContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public string CurrentFilter { get; set; }

        public PaginatedList<GigIndexModel> Gigs { get; set; }

        public class GigIndexModel
        {
            public int ID { get; set; }
            public string ArtistName { get; set; }
            public string VenueName { get; set; }
            public DateTime Date { get; set; }
            public Decimal TicketPrice { get; set; }
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

            var pageSize = Configuration.GetValue("PageSize", 4);
            Gigs = await PaginatedList<GigIndexModel>.CreateAsync(GigsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
