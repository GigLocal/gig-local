namespace GigLocal.Options;

public class ScraperOptions
{
    public ScraperOption[] Scrapers { get; set; }
}

public class ScraperOption
{
    public string Url { get; set; }
    public string Scraper { get; set; }
    public int VenueId { get; set; }
}
