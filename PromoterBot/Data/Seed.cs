using Microsoft.EntityFrameworkCore;
using PromoterBot.Models;

namespace PromoterBot.Data
{
    public static class Seed
    {
        public static void SeedInitialData(ModelBuilder builder)
        {
            var tashkent = new Region
            {
                Id = 1,
                Name = "Ташкентская область"
            };

            var regions = new List<Region>
            {
                tashkent,
                new Region
                {
                    Id = 2,
                    Name = "Самаркандская область"
                },
                new Region
                {
                    Id = 3,
                    Name = "Андижанская область"
                },
            };

            builder.Entity<Region>().HasData(regions);

            var cities = new List<City>
            {
                new City
                {
                    Id = 1,
                    Name = "Ташкент",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 2,
                    Name = "Газалкент",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 3,
                    Name = "Ангрен",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 4,
                    Name = "Алмалык",
                    RegionId = tashkent.Id
                }
                ,new City
                {
                    Id = 5,
                    Name = "Чирчик",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 6,
                    Name = "Кибрай",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 7,
                    Name = "Лолшахар",
                    RegionId = tashkent.Id
                },
            };

            builder.Entity<City>().HasData(cities);
        }
    }
}
