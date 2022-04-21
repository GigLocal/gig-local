namespace GigLocal.Pages.Admin;

public class IndexModel : BasePageModel
{
    public IndexModel(MetaTagService metaTagService) : base(metaTagService)
    {
    }

    public void OnGet()
    {
        ViewData["Title"] = "Admin";
        ViewData["Description"] = "Administer gigs and venues on Gig Local admin.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = $"{HttpContext.Request.GetDisplayUrl()}/";
    }
}
