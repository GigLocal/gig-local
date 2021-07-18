using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GigLocal.Data;
using GigLocal.Models;
using System.ComponentModel.DataAnnotations;

namespace GigLocal.Pages.Artists
{
    public class CreateModel : PageModel
    {
        private readonly GigContext _context;

        public CreateModel(GigContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ArtistCreateModel Artist { get; set; }

        public class ArtistCreateModel
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var newArtist = new Artist
            {
                Name = Artist.Name,
                Description = Artist.Description,
                Genre = Artist.Genre,
                Website = Artist.Website
            };

            _context.Artists.Add(newArtist);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
