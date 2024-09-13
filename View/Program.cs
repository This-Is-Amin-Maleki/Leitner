using DataAccessLeit.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ModelsLeit.DTOs.Notification;
using ModelsLeit.Entities;
using ServicesLeit.Interfaces;
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
                .AddIdentity<ApplicationUser, IdentityRole<long>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<CollectionService>();
            builder.Services.AddScoped<CardService>();
            builder.Services.AddScoped<BoxService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddTransient<FileService>();//stateless Service
            // using Notification Service 
            builder.Services.Configure<SmtpServiceDto>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddTransient<NotificationService>(serviceProvider =>
            {
                var smtpServiceDto = serviceProvider.GetRequiredService<IOptions<SmtpServiceDto>>().Value;
                return new NotificationService(smtpServiceDto);
            });

            // using Microsoft.AspNetCore.Identity;

            builder.Services.Configure<PasswordHasherOptions>(option =>
            {
                option.IterationCount = 12000;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/";
                options.Cookie.Name = "Leitner";
                options.LoginPath = "/";
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            
            // Force Identity's security stamp to be validated every hour.
            builder.Services.Configure<SecurityStampValidatorOptions>(options => 
                               options.ValidationInterval = TimeSpan.FromHours(1));

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = false;

                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                // Default Password settings.
                //options.Password.RequireDigit = true;
                //options.Password.RequireLowercase = true;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireUppercase = true;
                //options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;

                // Default SignIn settings.
                //options.SignIn.RequireConfirmedEmail = false;
                //options.SignIn.RequireConfirmedPhoneNumber = false;

                // Default Register settings.
                //options.User.AllowedUserNameCharacters =
          //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //options.User.RequireUniqueEmail = false;

            }
            );
            //token validation duration
            builder.Services.Configure<DataProtectionTokenProviderOptions>( options => 
                options.TokenLifespan = TimeSpan.FromDays(1));


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/");
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
                pattern: "{controller=Page}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
