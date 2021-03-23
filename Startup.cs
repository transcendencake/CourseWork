using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[] { new CultureInfo("en"), new CultureInfo("ru") };
                options.DefaultRequestCulture = new RequestCulture("ru");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            StorageUtils.SetDefaults(Configuration.GetConnectionString("BlobConnection"), "pictures");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                 {
                     googleOptions.ClientId = Configuration["Oauth:googleId"];
                     googleOptions.ClientSecret = Configuration["Oauth:googleSecret"];
                     googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
                 })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Oauth:facebookId"];
                    facebookOptions.AppSecret = Configuration["Oauth:facebookSecret"];
                    facebookOptions.SignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = Configuration["Oauth:microsoftId"];
                    microsoftOptions.ClientSecret = Configuration["Oauth:microsoftSecret"]; ;
                    microsoftOptions.SignInScheme = IdentityConstants.ExternalScheme;
                });
                 
             
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");              
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<Hubs.CommentsHub>("/comments");
                endpoints.MapRazorPages();
            });
        }
    }
}
