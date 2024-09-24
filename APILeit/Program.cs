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

            // Configure services and DI
            builder.Services.AddScoped<ITokenService, TokenService>();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
            app.UseHttpsRedirection();

                    app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}