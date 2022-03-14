namespace GigLocal.Services;

record SlackMessage
(
    string Text
);

public interface ISlackService
{
    Task PostGigUploadedMessageAsync(int gigId);
}

public class SlackService : ISlackService
{
    private readonly HttpClient _httpClient;
    private readonly SlackOptions _options;
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _httpContext;

    public SlackService(
        HttpClient httpClient,
        IOptions<SlackOptions> options,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _linkGenerator = linkGenerator;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task PostGigUploadedMessageAsync(int gigId)
    {
        var link = _linkGenerator.GetUriByPage(
            _httpContext,
            "/Admin/Gigs/Approve",
            values: new { id = gigId },
            scheme: "https"
        );
        var result = await _httpClient.PostAsJsonAsync<SlackMessage>(
            _options.GigUploadWebhook,
            new SlackMessage(
                $"A new gig has been uploaded! {link}"
            )
        );

        result.EnsureSuccessStatusCode();
    }
}
