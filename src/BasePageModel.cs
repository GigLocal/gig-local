namespace GigLocal;

public abstract class BasePageModel : PageModel
{
    protected readonly MetaTagService MetaTagService;

    public BasePageModel(MetaTagService metaTagService)
    {
        MetaTagService = metaTagService;
    }
}
