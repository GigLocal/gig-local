﻿namespace GigLocal.Pages;

public class GigsModel : PageModel
{
    private readonly GigContext _context;

    public IEnumerable<GigRecord> Gigs { get; set; }

    public GigsModel(GigContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(14);
        var gigsQuery = _context.Gigs
            .AsNoTracking()
            .Include(g => g.Venue)
            .Where(g => g.Approved
                        && g.Date >= startDate
                        && g.Date <= endDate)
            .OrderBy(g => g.Date);

        var gigs = await gigsQuery.ToArrayAsync();

        Gigs = gigsQuery.Select(g => new GigRecord(
            g.Date.ToDayOfWeekDateMonthName(),
            g.Date.ToTimeHourMinuteAmPm(),
            g.ArtistName,
            g.Description,
            g.EventUrl,
            g.ImageUrl,
            VenueHelper.GetFormattedNameLocation(g.Venue.Name, g.Venue.Suburb, g.Venue.State)
            ));
    }
}

public record GigRecord
(
    string Date,
    string Time,
    string ArtistName,
    string Description,
    string EventUrl,
    string Image,
    string VenueName
);
