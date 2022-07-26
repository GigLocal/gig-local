namespace GigLocal.Scrapers;

public record ScrapedGig(
    string EventTitle,
    DateTime StartDate,
    DateTime EndDate,
    string ImageUrl,
    string EventUrl,
    string Description
);
