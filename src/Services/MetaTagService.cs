namespace GigLocal.Services;

public class MetaTagService
{
    public readonly string IndexUrl;
    public readonly string LogoUrl;
    public readonly string VenuesUrl;
    public readonly string GigsUrl;
    public readonly string UploadUrl;
    public readonly string AboutUrl;
    public readonly string PrivacyUrl;
    public readonly string ErrorUrl;
    public readonly string AccessDeniedUrl;

    public MetaTagService(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var scheme = "https";


        IndexUrl = $"{linkGenerator.GetUriByPage(httpContext, "/Index", scheme: scheme)}";
        LogoUrl = $"{IndexUrl}logo.svg";
        VenuesUrl = $"{linkGenerator.GetUriByPage(httpContext, "/Venues", scheme: scheme)}/";
        GigsUrl = $"{linkGenerator.GetUriByPage(httpContext, "/Gigs", scheme: scheme)}/";
        UploadUrl = $"{linkGenerator.GetUriByPage(httpContext, "/Upload", scheme: scheme)}/";
        AboutUrl = $"{linkGenerator.GetUriByPage(httpContext, "/About", scheme: scheme)}/";
        PrivacyUrl = $"{linkGenerator.GetUriByPage(httpContext, "/Privacy", scheme: scheme)}/";
        ErrorUrl = $"{linkGenerator.GetUriByPage(httpContext, "/Error", scheme: scheme)}/";
        AccessDeniedUrl = $"{linkGenerator.GetUriByPage(httpContext, "/AccessDenied", scheme: scheme)}/";
    }
}
