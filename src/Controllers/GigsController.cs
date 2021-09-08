using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GigLocal.Data;
using GigLocal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Standards.AspNetCore;

namespace GigLocal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GigsController : ControllerBase
    {
        private readonly GigContext _context;

        public GigsController(GigContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<GigList> Get(
            [Required, ModelBinder(typeof(IsoDateModelBinder))] DateTime startDate,
            [Required, ModelBinder(typeof(IsoDateModelBinder))] DateTime endDate,
            [Required] string location,
            [Required, Range(1, int.MaxValue)] int page,
            [Required, Range(1, 50)] int pageSize)
        {
            var gigsQuery = _context.Gigs
                .AsNoTracking()
                .Include(g => g.Artist)
                .Include(g => g.Venue)
                .Where(g => g.Date >= startDate
                    && g.Date <= endDate
                    && EF.Functions.Like(g.Venue.Address, $"%{location}%"))
                .OrderBy(g => g.Date);

            var gigsQueryResult = await PaginatedList<Gig>.CreateAsync(gigsQuery, page, pageSize);

            var gigs = gigsQueryResult.Select(g => new GigRecord(
                    g.Date.ToDayOfWeekDateMonthName(),
                    g.Date.ToTimeHourMinuteAmPm(),
                    g.TicketPrice,
                    g.TicketWebsite,
                    g.Artist.Name,
                    g.Artist.Genre,
                    g.Artist.Website,
                    g.Artist.ImageUrl,
                    g.Venue.Name,
                    g.Venue.Website,
                    g.Venue.Address));

            return new GigList(gigs, gigsQueryResult.TotalPages);
        }
    }

    public record GigRecord
    (
        string Date,
        string Time,
        Decimal TicketPrice,
        string TicketWebsite,
        string ArtistName,
        string ArtistGenre,
        string ArtistWebsite,
        string ArtistImage,
        string VenueName,
        string VenueWebsite,
        string VenueAddress
    );

    public record GigList
    (
        IEnumerable<GigRecord> Gigs,
        int TotalPages
    );
}
