using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static void MigrateDatabase<TContext>(IApplicationBuilder builder, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using var serviceScope = builder.ApplicationServices.CreateScope();
            var service = serviceScope.ServiceProvider;
            var _logger = service.GetRequiredService<ILogger<TContext>>();
            var _context = service.GetService<TContext>();
            var retryForAvailability = 0;
        
  

                try
                {
                _logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);                  
                    InvokeSeeder(seeder, _context, service);

                _logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (SqlException ex)
                {
                _logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    if (retryForAvailability<50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(builder, seeder);
                    }
                }
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
