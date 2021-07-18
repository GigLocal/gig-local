using GigLocal.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using GigLocal.Data;

namespace GigLocal.Pages.Venues
{
    public class IndexModel : PageModel
    {
        private readonly GigContext _context;

        public IndexModel(GigContext context)
        {
            _context = context;
        }

        public IList<Venue> Venues { get; set; }

        public async Task OnGetAsync()
        {
            Venues = await _context.Venues
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
