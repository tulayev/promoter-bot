using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Services.Cloudinary;

namespace PromoterBot.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => {
                options.UseSqlite(
                    config.GetConnectionString("Default")
                );
            });

            services.AddAutoMapper(typeof(Program).Assembly);
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

            return services;
        }
    }
}
