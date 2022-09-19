using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;

namespace PromoterBot.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => {
                options.UseNpgsql(GetConnectionString(config));
            });

            services.AddAutoMapper(typeof(Program).Assembly);

            return services;
        }

        // Get Heroku Postgres conn string
        private static string GetConnectionString(IConfiguration config)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                return config.GetConnectionString("Default");


            string connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            var databaseUri = new Uri(connectionUrl);
            string db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
        }
    }
}
