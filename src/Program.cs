var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<GigContext>(
    options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddCookie(options => {
    options.AccessDeniedPath = new PathString("/AccessDenied");
}).AddGoogle(options =>
{
    options.ClientId = configuration["Authentication:Google:ClientId"];
    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddAuthorization(options =>
{
    // Temporary for now, until we have a better way to handle this
    options.AddPolicy("AllowedUsersOnly", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email is null)
                return false;

            var adminEmails = configuration["Authentication:Admin:Emails"].Split(',');
            return adminEmails.Contains(email);
        });
    });
});

builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
});

var mvcBuilder = builder.Services.AddRazorPages(options => {
    options.Conventions.AuthorizeFolder("/Admin", "AllowedUsersOnly");
    options.Conventions.AddPageRoute("/GigsByVenue", "venues/{venueId}/{venueName}");
    options.Conventions.AddPageRoute("/Gig", "gigs/{gigId}/{gigName}");
});

if (environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}
else {
    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.Configure<ScraperOptions>(configuration.GetSection("Scraper"));
    services.AddHostedService<ScraperService>();
}

builder.Services.Configure<StorageOptions>(configuration.GetSection("Storage"));
builder.Services.Configure<RecaptchaOptions>(configuration.GetSection("Recaptcha"));
builder.Services.Configure<SlackOptions>(configuration.GetSection("Slack"));
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<MetaTagService>();
builder.Services.AddScoped<ISiteMapService, SitemapService>();
builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();
builder.Services.AddHttpClient<ISlackService, SlackService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromDays(1)));
    options.AddPolicy("NoCache", builder => builder.SetVaryByRouteValue("admin").NoCache());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseOutputCache();

app.UseAuthentication();

app.UseAuthorization();

app.UseResponseCompression();

app.MapRazorPages();

app.Run();
