using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using GigLocal.Data;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using GigLocal.Services;

namespace GigLocal
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true)
                    .AddDbContext<GigContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.AccessDeniedPath = new PathString("/AccessDenied");
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
            services.AddAuthorization(options =>
            {
                // Temporary for now, until we have a better way to handle this
                options.AddPolicy("AllowedUsersOnly", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                        return email is not null && email.Equals("giglocallive@gmail.com");
                    });
                });
            });

            var mvcBuilder = services.AddRazorPages(options => {
                options.Conventions.AuthorizeFolder("/Admin", "AllowedUsersOnly");
            });

            if (Environment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            services.AddApplicationInsightsTelemetry();

            services.Configure<StorageOptions>(Configuration.GetSection("Storage"));
            services.AddSingleton<IStorageService, StorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
