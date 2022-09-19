using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PromoterBot.Models.Configurations
{
    public class ParticipantConfig : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.Property(x => x.PromoterId).IsRequired();
            builder.Property(x => x.City).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.FavouriteBrands).IsRequired().HasMaxLength(255);
            builder.Property(x => x.SocialNetwork).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Age).IsRequired();
            builder.Property(x => x.Gender).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Image).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        }
    }
}
