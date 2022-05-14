namespace GigLocal.Services;

public interface ISiteMapService
{
    Task<string> GetSitemapAsync();
}

public class SitemapService : ISiteMapService
{
    private const string Scheme = "https";
    private readonly MetaTagService _metaTagService;
    private readonly GigContext _context;

    public SitemapService(
        MetaTagService metaTagService,
        GigContext context
    )
    {
        _context = context;
        _metaTagService = metaTagService;
    }

    public async Task<string> GetSitemapAsync()
    {
        // Top level URLs
        var urls = new List<string>
        {
            _metaTagService.GigsUrl,
            _metaTagService.VenuesUrl,
            _metaTagService.UploadUrl,
            _metaTagService.AboutUrl,
            _metaTagService.PrivacyUrl
        };

        // Venues
        var venueNames = await _context.Venues
                                 .Select(v => new { v.ID, v.Name, v.Suburb, v.State })
                                 .ToListAsync();
        var venueUrls = venueNames.Select(v => $"{_metaTagService.IndexUrl}{VenueHelper.GetUrlFriendlyName(v.ID, v.Name, v.Suburb, v.State)}");
        urls.AddRange(venueUrls);

        // Sitemap document
        var sitemap = new XmlDocument();
        var declaration = sitemap.CreateXmlDeclaration("1.0", "UTF-8", null);
        var root = sitemap.DocumentElement;
        sitemap.InsertBefore(declaration, root);

        var urlset = sitemap.CreateElement("urlset");
        urlset.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

        // Add URLs
        foreach (var url in urls)
        {
            var urlNode = sitemap.CreateElement("url");
            urlset.AppendChild(urlNode);
            var locNode = sitemap.CreateElement("loc");
            locNode.InnerText = url;
            urlNode.AppendChild(locNode);
        }

        sitemap.InsertAfter(urlset, declaration);

        return sitemap.OuterXml;
    }
}
