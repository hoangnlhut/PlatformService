using CommandService.Models;
using CommandService.Repository;
using CommandService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static async Task PrepPopulation(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
               var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>() ?? throw new ArgumentNullException("IPlatformDataClient");

                var platforms = await grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>() ?? throw new ArgumentNullException("ICommandRepository"), platforms);
            }
        }

        private static void SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("Seeding new platforms from Command Service.");

            foreach (var platform in platforms)
            {
                if (!commandRepository.ExternalPlatformExists(platform.ExternalID))
                {
                    commandRepository.CreatePlatform(platform);
                    Console.WriteLine("Adding platform: {0}", platform.Name);
                }
                else 
                {
                    Console.WriteLine("Existed platform: {0}", platform.Name);
                }
                commandRepository.SaveChanges();
            }

            Console.WriteLine("Seeding complete.");
        }
    }
}
