using Microsoft.EntityFrameworkCore;
using PromoterBot.Models;

namespace PromoterBot.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Promoter> Promoters { get; set; }
        
        public DbSet<Participant> Participants { get; set; }
    }
}
