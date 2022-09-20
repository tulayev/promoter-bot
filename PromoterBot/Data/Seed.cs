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

            var namangan = new Region
            {
                Id = 2,
                Name = "Наманганская область"
            };

            var ferghana = new Region
            {
                Id = 3,
                Name = "Ферганская область"
            };

            var samarkand = new Region
            {
                Id = 4,
                Name = "Самаркандская область"
            };

            var bukhara = new Region
            {
                Id = 5,
                Name = "Бухарская область"
            };

            var navoi = new Region
            {
                Id = 6,
                Name = "Навоийская область"
            };

            var andijan = new Region
            {
                Id = 7,
                Name = "Андижанская область"
            };

            var regions = new List<Region>
            {
                tashkent,
                namangan,
                ferghana,
                samarkand,
                bukhara,
                navoi,
                andijan
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
                    Name = "Янгиюль",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 3,
                    Name = "Назарбек",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 4,
                    Name = "Келес",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 5,
                    Name = "Чирчик",
                    RegionId = tashkent.Id
                },
                new City
                {
                    Id = 6,
                    Name = "Наманган",
                    RegionId = namangan.Id
                },
                new City
                {
                    Id = 7,
                    Name = "Фергана",
                    RegionId = ferghana.Id
                },
                new City
                {
                    Id = 8,
                    Name = "Самарканд",
                    RegionId = samarkand.Id
                },
                new City
                {
                    Id = 9,
                    Name = "Бухара",
                    RegionId = bukhara.Id
                },
                new City
                {
                    Id = 10,
                    Name = "Навои",
                    RegionId = navoi.Id
                },
                new City
                {
                    Id = 11,
                    Name = "Андижан",
                    RegionId = andijan.Id
                }
            };

            builder.Entity<City>().HasData(cities);
        }
    }
}
