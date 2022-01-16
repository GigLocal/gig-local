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

var mvcBuilder = builder.Services.AddRazorPages(options => {
    options.Conventions.AuthorizeFolder("/Admin", "AllowedUsersOnly");
});

if (environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<StorageOptions>(configuration.GetSection("Storage"));
builder.Services.AddSingleton<IImageService, ImageService>();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
