using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class CommandDBContextcs:DbContext
    {
        public CommandDBContextcs(DbContextOptions<CommandDBContextcs> options) : base(options)
        {
        }
        public DbSet<Command> Commands { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Command>()
                .HasOne(c => c.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(p => p.PlatformId);
            modelBuilder.Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(c => c.Platform)
                .HasForeignKey(c => c.PlatformId);
        }
    }
}
