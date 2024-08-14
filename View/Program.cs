using DataAccessLeit.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using ServicesLeit.Services;
using System.Diagnostics;
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

            builder.Services
                .AddIdentity<ApplicationUser, UserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            builder.Services.AddScoped<CollectionService>();
            builder.Services.AddScoped<CardService>();
            builder.Services.AddScoped<BoxService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddTransient<FileService>();//stateless Service
            
            builder.Services.ConfigureApplicationCookie(o =>
                o.AccessDeniedPath = "/"
            );

            builder.Services.Configure<IdentityOptions>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = true;
                x.Password.RequireUppercase = false;
                x.Lockout.MaxFailedAccessAttempts = 5;
                x.SignIn.RequireConfirmedEmail = false;
                
            }
            );
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(3));


            var app = builder.Build();

            
                app.UseExceptionHandler("/Error");
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.AutoMigrationAndSeedDataAsync<ApplicationDbContext>(
                builder.Configuration["Admin:Email"],
                builder.Configuration["Admin:Password"]
            );

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default", 
                //pattern: "{controller=Collection}/{action=Index}/{id?}");
                pattern: "{controller=Page}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
