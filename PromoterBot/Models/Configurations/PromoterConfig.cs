using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PromoterBot.Models.Configurations
{
    public class PromoterConfig : IEntityTypeConfiguration<Promoter>
    {
        public void Configure(EntityTypeBuilder<Promoter> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.City).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(50);
        }
    }
}
