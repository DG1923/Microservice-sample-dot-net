using CommandService.Models;
using CommandService.SyncDataService.Grpc;

namespace CommandService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatform();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo? commandRepo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms...");
            if(commandRepo == null)
            {
                Console.WriteLine("--> Could not find command repo");
                return;
            }
            if(platforms == null || !platforms.Any())
            {
                Console.WriteLine("--> No platforms were found");
                return;
            }
            foreach (var plat in platforms)
            {
                if (!commandRepo.ExternalPlatformExists(plat.ExternalID))
                {
                    commandRepo.CreatePlatform(plat);
                }
                commandRepo.SaveChanges();
                Console.WriteLine($"--> {plat.Name} was added to DB");
            }
        }
    }
}
