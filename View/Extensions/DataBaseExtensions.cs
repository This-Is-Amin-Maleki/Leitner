using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ModelsLeit.Entities;
using SharedLeit;
using System;
using System.Data;

namespace ViewLeit.Extensions
{
    public static class DataBaseExtensions
    {
        public static async void AutoMigrationAndSeedDataAsync<T>(this IHost host, string? email, string? password) where T : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<T>();

            //check require any migrations
            bool isMigrationNeeded = (await context.Database.GetPendingMigrationsAsync()).Any();
            if (isMigrationNeeded)
            {
                //update database by migration
                await context.Database.MigrateAsync();

                //seed All roles if found any role
                await SeedRolesAsync(services);

                //seed Default admin if not found Default admin
                if (email != null && password != null)
                {
                    await SeedAdminAsync(services, email, password);
                }
            }
        }

        private static async Task SeedAdminAsync(IServiceProvider services, string email, string password)
        {
            //admin role:
            const string roleName = nameof(UserType.Admin);

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            //find user
            var user = await userManager.FindByNameAsync(email);
            //user not found
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Active = true,
                    Bio = string.Empty,
                    Name = "Master",
                };
                //create Default Admin user
                await userManager.CreateAsync(user, password);
            }
            //add role to Admin
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
        private static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();

            //check exist any roles? Add all roles
            var countRoles = await roleManager.Roles.CountAsync();
            if (countRoles is 0)
            {
                foreach (var role in Enum.GetNames(typeof(UserType)))// Enum.GetValues(typeof(UserType)))
                {
                    IdentityRole<long> userRole = new (role);
                    //add admin role
                    await roleManager.CreateAsync(userRole);
                }
            }
        }
    }
}