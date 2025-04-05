using Microsoft.EntityFrameworkCore;
using PlatformService2.Models;


namespace PlatformService2.Data
{
    public class PlatformDbContext:DbContext
    {
        public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options)
        {
            
        }
        public DbSet<Platform> Platforms { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
