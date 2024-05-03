using Microsoft.EntityFrameworkCore;

namespace ViewLeit.Extensions
{
    public static class DataBaseExtensions
    {
        public static async void AutoMigration<T>(this IHost host) where T :DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<T>();
            context.Database.EnsureCreated();
        }
    }
}
