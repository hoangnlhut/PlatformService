using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>() ?? throw new InvalidOperationException("Unable to resolve AppDbContext"));
            }
        }

        private static void SeedData(AppDbContext appDbContext)
        {
            if(!appDbContext.Platforms.Any())
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
