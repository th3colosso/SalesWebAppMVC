using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SalesWebMVC.Data;
using SalesWebMVC.Services;
using System.Globalization;

namespace SalesWebMVC {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SalesWebMVCContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMVCContext"), 
                                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SalesWebMVCContext")), 
                                    builder => builder.MigrationsAssembly("SalesWebMVC")));
            builder.Services.AddScoped<SeedingService>();
            builder.Services.AddScoped<SellerService>();
            builder.Services.AddScoped<DepartmentService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var culture = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions {
                DefaultRequestCulture = new RequestCulture(culture),
                SupportedCultures = new List<CultureInfo> { culture },
                SupportedUICultures = new List<CultureInfo> { culture }
            };

            var app = builder.Build();

            app.UseRequestLocalization(localizationOptions);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            } else {
                using (var scope = app.Services.CreateScope()) {
                    var seeder = scope.ServiceProvider.GetRequiredService<SeedingService>();
                    seeder.Seed();
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }

    }
}