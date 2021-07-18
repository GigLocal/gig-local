using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GigLocal.Data;
using System.ComponentModel.DataAnnotations;

namespace GigLocal.Pages.Artists
{
    public class EditModel : PageModel
    {
        private readonly GigContext _context;

        public EditModel(GigContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ArtistEditModel Artist { get; set; }

        public class ArtistEditModel
        {
            [Required]
            [StringLength(50)]
            public string Name { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public string Genre { get; set; }

            [Required]
            public string Website { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            Artist = new ArtistEditModel
            {
                Name = artist.Name,
                Description = artist.Description,
                Genre = artist.Genre,
                Website = artist.Website
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
                return Page();

            var artistToUpdate = await _context.Artists.FindAsync(id);

            if (artistToUpdate == null)
            {
                return NotFound();
            }

            artistToUpdate.Name = Artist.Name;
            artistToUpdate.Description = Artist.Description;
            artistToUpdate.Genre = Artist.Genre;
            artistToUpdate.Website = Artist.Website;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
