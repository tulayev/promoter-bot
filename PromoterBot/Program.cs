using Deployf.Botf;
using PromoterBot.Extensions;

namespace PromoterBot
{
    public class Program : BotfProgram
    {
        public static void Main(string[] args)
        {
            StartBot(args, 
            onConfigure: (services, config) =>
            {
                services.AddApplicationServices(config);
            }, 
            onRun: async (app, config) =>
            {
                var castedApp = (WebApplication)app;

                await castedApp.MigrateDatabaseAsync();

                while (true)
                {
                    try
                    {
                        Thread.Sleep(2000 * 60);
                        await new HttpClient().GetStringAsync(config["BaseUrl"]);
                    }
                    catch (Exception ex)
                    {
                        if (ex is HttpRequestException)
                        {
                            continue;
                        }
                    }
                }
            });
        }
    }
}