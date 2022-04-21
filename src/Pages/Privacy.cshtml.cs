namespace GigLocal.Pages;

public class PrivacyModel : BasePageModel
{
    public PrivacyModel(MetaTagService metaTagService) : base(metaTagService)
    {
    }

    public void OnGet()
    {
        ViewData["Title"] = "Privacy Policy";
        ViewData["Description"] = "Privacy Policy of Gig Local.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = MetaTagService.PrivacyUrl;
    }
}
