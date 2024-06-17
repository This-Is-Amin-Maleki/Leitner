using DataAccessLeit.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using ServicesLeit.Services;
using ViewLeit.Extensions;

namespace ViewLeit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //log One Of Two
            builder.Logging.AddDebug();
            //builder.Host.ConfigureLogging(l =>
            //{
            //    l.AddDebug();
            //});

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<CollectionService>();
            builder.Services.AddScoped<CardService>();
            builder.Services.AddScoped<BoxService>();
            builder.Services.AddTransient<FileService>();//stateless Service

            builder.Services.ConfigureApplicationCookie(o =>
                o.AccessDeniedPath = "/"
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

            app.MapControllerRoute(
                name: "default", 
                //pattern: "{controller=Collection}/{action=Index}/{id?}");
                pattern: "{controller=Box}/{action=Index}/{id?}");
            app.AutoMigration<ApplicationDbContext>();
            app.Run();
        }
    }
}
