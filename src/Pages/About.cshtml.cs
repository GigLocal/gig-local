namespace GigLocal.Pages;

public class AboutModel : BasePageModel
{
    public AboutModel(MetaTagService metaTagService) : base(metaTagService)
    {
    }

    public void OnGet()
    {
        ViewData["Title"] = "About";
        ViewData["Description"] = "Gig Local is a not-for-profit community-driven website to share small music gigs at bars, pubs and cafes in your local area.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = MetaTagService.AboutUrl;
    }
}
