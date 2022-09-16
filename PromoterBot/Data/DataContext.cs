using Microsoft.EntityFrameworkCore;
using PromoterBot.Models;

namespace PromoterBot.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
            Seed.SeedInitialData(modelBuilder);
        }

        public DbSet<Promoter> Promoters { get; set; }
        
        public DbSet<Participant> Participants { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<City> Cities { get; set; }
    }
}
