using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(this WebApplication app)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>() ?? throw new InvalidOperationException("Unable to resolve AppDbContext"), app.Environment);
            }
        }

        private static void SeedData(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            if (environment.IsProduction())
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    appDbContext.Database.Migrate(); //we need to apply migrations in production, otherwise we will get an error when trying to run the app
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("--> Using In-Memory Database, no migrations needed.");

                if (!appDbContext.Platforms.Any())
                {
                    Console.WriteLine("Seeding data...");

                    List<Platform> platforms = new List<Platform>
                {
                    new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                };

                    appDbContext.Platforms.AddRange(platforms);
                    appDbContext.SaveChanges();

                    Console.WriteLine("Seeding complete.");
                }
                else
                {
                    Console.WriteLine("We already have data");
                }
            }
        }
    }
}
