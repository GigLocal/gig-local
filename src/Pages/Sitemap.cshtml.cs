namespace GigLocal.Pages;

public class SitemapModel : PageModel
{
    private readonly ISiteMapService _siteMapService;
    private readonly IWebHostEnvironment _env;

    public SitemapModel(ISiteMapService siteMapService, IWebHostEnvironment env)
    {
        _siteMapService = siteMapService;
        _env = env;
    }

    public async Task<IActionResult> OnGet()
    {
        if (!_env.IsProduction())
        {
            return NotFound();
        }

		return new ContentResult
		{
			ContentType = MediaTypeNames.Text.Xml,
			Content = await _siteMapService.GetSitemapAsync(),
			StatusCode = 200
		};
    }
}
