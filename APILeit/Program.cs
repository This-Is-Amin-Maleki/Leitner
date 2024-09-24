using DataAccessLeit.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ServicesLeit.Interfaces;
using ServicesLeit.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModelsLeit.DTOs.Notification;
using ModelsLeit.Entities;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace APILeit
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging configuration            Just One Of Two
            builder.Logging.AddDebug();
            //builder.Host.ConfigureLogging(l =>
            //{
            //    l.AddDebug();
            //});

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Leitner", Version = "1" });

                options.AddSecurityDefinition(
                    JwtBearerDefaults.AuthenticationScheme,//"Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type =   SecuritySchemeType.ApiKey,//SecuritySchemeType.Http,
                        Scheme =   JwtBearerDefaults.AuthenticationScheme,//"Bearer", //
                        BearerFormat = "JWT",
                        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer abcdef12345\""
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme ,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme =   "Oauth2" ,//"Bearer",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In=ParameterLocation.Header
                    },
                    new List <string> ()
                }
                });
            });


            //builder.Services.AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Program>());




            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Identity and Authentication configuration
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole<long>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<PasswordHasherOptions>(option =>
            {
                option.IterationCount = 12000;
            });

            // Configure services and DI
            builder.Services.AddScoped<CollectionService>();
            builder.Services.AddScoped<CardService>();
            builder.Services.AddScoped<BoxService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddTransient<FileService>(); // Stateless service

            // using Notification Service 
            builder.Services.Configure<SmtpServiceDto>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddTransient<NotificationService>(serviceProvider =>
            {
                var smtpServiceDto = serviceProvider.GetRequiredService<IOptions<SmtpServiceDto>>().Value;
                return new NotificationService(smtpServiceDto);
            });

            // JWT Configuration
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });


            builder.Services.ConfigureApplicationCookie(options =>
            {/*
                options.AccessDeniedPath = "/";
                options.Cookie.Name = "Leitner";
                options.LoginPath = "/";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;/*
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {*/
              
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });
            /*
            // Force Identity's security stamp to be validated every hour.
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
                               options.ValidationInterval = TimeSpan.FromHours(1));
    */
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
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromDays(1));


            var app = builder.Build();

            // Enable Swagger in Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Middleware for request pipeline
            app.UseHttpsRedirection();

                    app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}