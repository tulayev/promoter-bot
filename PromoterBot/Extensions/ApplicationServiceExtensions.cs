using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;

namespace PromoterBot.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => {
                options.UseSqlite(config.GetConnectionString("Default"));
            });

            return services;
        }
    }
}
