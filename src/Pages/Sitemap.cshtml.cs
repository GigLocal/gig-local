namespace GigLocal.Pages;

public class SitemapModel : PageModel
{
    private readonly ISiteMapService _siteMapService;

    public SitemapModel(ISiteMapService siteMapService)
    {
        _siteMapService = siteMapService;
    }

    public async Task<IActionResult> OnGet()
    {
		return new ContentResult
		{
			ContentType = MediaTypeNames.Text.Xml,
			Content = await _siteMapService.GetSitemapAsync(),
			StatusCode = 200
		};
    }
}
