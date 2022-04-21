namespace GigLocal.Pages;

public class IndexModel : BasePageModel
{
    public IndexModel(MetaTagService metaUrlService) : base(metaUrlService)
    {
    }

    public void OnGet()
    {
        ViewData["Title"] = "Find local live music gigs near you";
        ViewData["Description"] = "Gig Local is a not-for-profit community-driven website to share small music gigs at bars, pubs and cafes in your local area.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = MetaTagService.IndexUrl;
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage();
    }
}
