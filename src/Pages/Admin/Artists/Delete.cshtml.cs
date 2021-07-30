using GigLocal.Data;
using GigLocal.Models;
using GigLocal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GigLocal.Pages.Admin.Artists
{
    public class DeleteModel : PageModel
    {
        private readonly GigContext _context;
        private readonly ILogger<DeleteModel> _logger;
        private readonly IStorageService _storageService;

        public DeleteModel(GigContext context,
                           ILogger<DeleteModel> logger,
                           IStorageService storageService)
        {
            _context = context;
            _logger = logger;
            _storageService = storageService;
        }

        public string ErrorMessage { get; set; }

        public ArtistDeleteModel Artist { get; set; }

        public class ArtistDeleteModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string Genre { get; set; }

            public string Website { get; set; }

            public string ImageUrl { get; set; }
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
                    Website = artist.Website,
                    ImageUrl = artist.ImageUrl
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
                await _storageService.DeleteArtistImageAsync(artist.ID);
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
