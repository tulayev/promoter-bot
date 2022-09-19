using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PromoterBot.Models.Configurations
{
    public class RegionConfig : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}
