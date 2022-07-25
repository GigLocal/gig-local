using System.Net;
using GigLocal.Scrapers;

namespace GigLocal.Services;

public class ScraperService : IHostedService, IDisposable
{
    private System.Timers.Timer _timer;
    private readonly CronExpression _expression;
    private readonly TimeZoneInfo _timeZoneInfo;
    private readonly ScraperOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IImageService _imageService;
    private readonly IServiceScopeFactory _scopeFactory;

    public ScraperService(
        IOptions<ScraperOptions> optionsAccessor,
        IHttpClientFactory httpClientFactory,
        IImageService imageService,
        IServiceScopeFactory scopeFactory)
    {
        _expression = CronExpression.Parse("0 3 * * *");
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Australia/Melbourne");
        _options = optionsAccessor.Value;
        _httpClient = httpClientFactory.CreateClient("ScraperService");
        _imageService = imageService;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ScheduleJob(cancellationToken);
    }

    public async Task ScheduleJob(CancellationToken cancellationToken)
    {
        var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
        if (next.HasValue)
        {
            var delay = next.Value - DateTimeOffset.Now;
            if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
            {
                await ScheduleJob(cancellationToken);
            }
            _timer = new System.Timers.Timer(delay.TotalMilliseconds);
            _timer.Elapsed += async (sender, args) =>
            {
                _timer.Dispose();  // reset and dispose timer
                _timer = null;

                if (!cancellationToken.IsCancellationRequested)
                {
                    await DoWork(cancellationToken);
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    await ScheduleJob(cancellationToken);    // reschedule next
                }
            };
            _timer.Start();
        }
        await Task.CompletedTask;
    }

    public async Task DoWork(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GigContext>();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
        foreach (var option in _options.Scrapers)
        {
            if (option.Scraper == nameof(SquarespaceScraper))
            {
                var scraper = new SquarespaceScraper(option.Url);
                var html = await _httpClient.GetStringAsync(option.Url);
                var scrapedGigs = scraper.Scrape(html);
                foreach (var scrapedGig in scrapedGigs)
                {
                    var foundGig = await context.Gigs
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(g => g.EventUrl == scrapedGig.EventUrl);
                    if (foundGig is not null)
                    {
                        continue;
                    }
                    var image = await _httpClient.GetByteArrayAsync(scrapedGig.ImageUrl);
                    using var memStream = new MemoryStream(image);
                    var imageUrl = await _imageService.UploadImageAsync(memStream);
                    Gig gig = new Gig
                    {
                        ArtistName = scrapedGig.EventTitle,
                        Description = scrapedGig.Description.Truncate(300),
                        VenueID = option.VenueId,
                        StartDate = scrapedGig.StartDate,
                        EndDate = scrapedGig.EndDate,
                        EventUrl = scrapedGig.EventUrl,
                        ImageUrl = imageUrl,
                        Approved = true
                    };
                    await context.AddAsync(gig);
                }
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        await Task.CompletedTask;
    }

    public virtual void Dispose()
    {
        _timer?.Dispose();
    }
}
