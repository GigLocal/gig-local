namespace GigLocal.Options;

public class ScraperOptions
{
    public ScraperOption[] Scrapers { get; set; }
}

public class ScraperOption
{
    public string BaseUrl { get; set; }
    public string GigPath { get; set; }
    public string GigUrl => $"{BaseUrl}/{GigPath}";
    public string Scraper { get; set; }
    public int VenueId { get; set; }
}
