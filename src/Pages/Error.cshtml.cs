namespace GigLocal.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : BasePageModel
{
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public ErrorModel(ILogger<ErrorModel> logger, MetaTagService metaTagService) : base(metaTagService)
    {
    }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        ViewData["Title"] = "Error";
        ViewData["Description"] = "Whoops, something has gone wrong.";
        ViewData["Image"] = MetaTagService.LogoUrl;
        ViewData["Url"] = MetaTagService.ErrorUrl;
    }
}
