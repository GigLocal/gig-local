using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GigLocal.Data;
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
        public async Task<IEnumerable<GigRecord>> Get(
            [Required, ModelBinder(typeof(IsoDateModelBinder))] DateTime startDate,
            [Required, ModelBinder(typeof(IsoDateModelBinder))] DateTime endDate,
            [Required] string location,
            [Required, Range(0, int.MaxValue)] int page,
            [Required, Range(0, 50)] int pageSize)
        {

            var gigsQuery = _context.Gigs
                .AsNoTracking()
                .Include(g => g.Artist)
                .Include(g => g.Venue)
                .Where(g => g.Date >= startDate
                    && g.Date <= endDate
                    && EF.Functions.Like(g.Venue.Address, $"%{location}%"));

            var queryResult = await gigsQuery
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
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return queryResult.Select(g => new GigRecord(
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
                    g.VenueAddress));
        }

        [HttpGet("pages")]
        public async Task<GigsInfo> GetCount(
            [Required, ModelBinder(typeof(IsoDateModelBinder))] DateTime startDate,
            [Required, ModelBinder(typeof(IsoDateModelBinder))] DateTime endDate,
            [Required] string location,
            [Required, Range(0, 50)] int pageSize)
        {
            var gigsQuery = _context.Gigs
                .AsNoTracking()
                .Include(g => g.Venue)
                .Where(g => g.Date >= startDate
                    && g.Date <= endDate
                    && EF.Functions.Like(g.Venue.Address, $"%{location}%"));

            var gigsCount = await gigsQuery.CountAsync();
            var pages = (int)Math.Ceiling(gigsCount / (double) pageSize);

            return new GigsInfo(pages);
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

    public record GigsInfo(int Pages);
}
