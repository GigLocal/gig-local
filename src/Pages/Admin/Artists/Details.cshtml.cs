using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GigLocal.Data;
using GigLocal.Models;
using System.ComponentModel.DataAnnotations;

namespace GigLocal.Pages.Admin.Artists
{
    public class DetailsModel : PageModel
    {
        private readonly GigContext _context;

        public DetailsModel(GigContext context)
        {
            _context = context;
        }

        public ArtistDetialsModel Artist { get; set; }

        public class ArtistDetialsModel
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string Genre { get; set; }

            public string Website { get; set; }

            [Display(Name = "Image")]
            public string ImageUrl { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!ModelState.IsValid)
                return Page();

            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(m => m.ID == id);

            if (artist == null)
            {
                return NotFound();
            }

            Artist = new ArtistDetialsModel
            {
                ID = artist.ID,
                Name = artist.Name,
                Description = artist.Description,
                Genre = artist.Genre,
                Website = artist.Website,
                ImageUrl = artist.ImageUrl
            };

            return Page();
        }
    }
}
