using Microsoft.EntityFrameworkCore;
using PlatformService2.Models;

namespace PlatformService2.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly PlatformDbContext _context;
        public PlatformRepo(PlatformDbContext context) {
            _context = context;
        }
        public async Task CreatePlatform(Platform plat)
        {
            if(plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            if(await _context.Platforms.ContainsAsync(plat))
            {
                throw new ArgumentException("Platform already exists");
            }
            await _context.Platforms.AddAsync(plat);
        }

        public async Task DeletePlatform(Platform plat)
        {
            if(plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            if(await _context.Platforms.ContainsAsync(plat))
            {
                _context.Platforms.Remove(plat);

            }
            else
            {
                throw new Exception("Platform not found");  
            }
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync()
        {
            return await _context.Platforms.ToListAsync();
        }

        public async Task<Platform> GetPlatformByIdAsync(int id)
        {
            if(id < 0)
            {
                throw new ArgumentException(nameof(id));
            }
            var platform =await _context.Platforms.FirstOrDefaultAsync(p => p.Id == id);
            if (platform == null) { 
                throw new Exception("Platform not found");
            }
            else
            {
                return platform;    
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync()>=0;
        }

        public async Task UpdatePlatform(Platform plat)
        {
            if(plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            if (await _context.Platforms.ContainsAsync(plat))
            {
                _context.Platforms.Update(plat);
            }
            else
            {
                throw new Exception("Platform not found");
            }
        }
    }

}
