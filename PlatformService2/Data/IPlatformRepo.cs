using PlatformService2.Models;

namespace PlatformService2.Data
{
    public interface IPlatformRepo
    {
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Platform>> GetAllPlatformsAsync();
        IEnumerable<Platform> GetAllPlatforms();
        Task<Platform> GetPlatformByIdAsync(int id);
        Task CreatePlatform(Platform plat);
        Task UpdatePlatform(Platform plat);
        Task DeletePlatform(Platform plat);
    }
}
