namespace GigLocal.Pages;

public class AccessDeniedModel : BasePageModel
{
    public AccessDeniedModel(MetaTagService metaTagService) : base(metaTagService)
    {
    }

    public void OnGet()
    {
        ViewData["Title"] = "Access Denied";
        ViewData["Description"] = "You're not allowed to access that.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = MetaTagService.AccessDeniedUrl;
    }
}
