using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GigLocal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Standards.AspNetCore;

namespace GigLocal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GigsController : ControllerBase
    {
        private readonly ILogger<GigsController> _logger;
        private readonly GigContext _context;
        private readonly int _pageSize;

        public GigsController(ILogger<GigsController> logger,
            GigContext context)
        {
            _logger = logger;
            _context = context;
            _pageSize = 20;
        }

        [HttpGet]
        public async Task<ActionResult<GigList>> Get(
            [Required] [ModelBinder(typeof(IsoDateModelBinder))] DateTime startDate,
            [Required] [ModelBinder(typeof(IsoDateModelBinder))] DateTime endDate,
            [Required] string stateSuburb,
            int page)
        {
            var gigCount = await _context.Gigs
                .Where(g => g.Date >= startDate && g.Date <= endDate)
                .CountAsync();

            var pages = (int)Math.Ceiling(gigCount / (double)_pageSize);

            var query = _context.Gigs
                .Include(g => g.Artist)
                .Include(g => g.Venue)
                .Where(g => g.Date >= startDate && g.Date <= endDate);

            var queryResult = await query
                .Select(g => new
                {
                    Date = g.Date,
                    TicketPrice = g.TicketPrice,
                    TicketWebsite = g.TicketWebsite,
                    ArtistName = g.Artist.Name,
                    ArtistGenre = g.Artist.Genre,
                    ArtistWebsite = g.Artist.Website,
                    ArtistImage = g.Artist.ImageUrl,
                    VenueName = g.Venue.Name,
                    VenueWebsite = g.Venue.Website,
                    VenueAddress = g.Venue.Address
                })
                .OrderBy(g => g.Date)
                .Skip(page * _pageSize)
                .Take(_pageSize)
                .ToArrayAsync();

            return new GigList(
                pages,
                queryResult.Select(g => new GigRecord(
                    g.Date.ToDayOfWeekDateMonthName(),
                    g.Date.ToTimeHourMinuteAmPm(),
                    g.TicketPrice,
                    g.TicketWebsite,
                    g.ArtistName,
                    g.ArtistGenre,
                    g.ArtistWebsite,
                    g.ArtistImage,
                    g.VenueName,
                    g.VenueWebsite,
                    g.VenueAddress)
                )
            );
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
        string VenueAddres
    );

    public record GigList
    (
        int Pages,
        IEnumerable<GigRecord> Gigs
    );
}
