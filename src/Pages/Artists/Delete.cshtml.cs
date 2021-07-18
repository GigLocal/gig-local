using GigLocal.Data;
using GigLocal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GigLocal.Pages.Artists
{
    public class DeleteModel : PageModel
    {
        private readonly GigContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(GigContext context,
                           ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string ErrorMessage { get; set; }

        public ArtistDeleteModel Artist { get; set; }

        public class ArtistDeleteModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string Genre { get; set; }

            public string Website { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Artist artist = await _context.Artists
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (artist == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = String.Format("Delete {ID} failed. Try again", id);
            }
            else
            {
                Artist = new ArtistDeleteModel
                {
                    Name = artist.Name,
                    Description = artist.Description,
                    Genre = artist.Genre,
                    Website = artist.Website
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
                return Page();

            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);

            if (artist == null)
            {
                return NotFound();
            }

            try
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ErrorMessage);

                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
