using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;

namespace PromoterBot.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => {
                options.UseNpgsql(config.GetConnectionString("Default"));
            });

            services.AddAutoMapper(typeof(Program).Assembly);

            return services;
        }
    }
}
