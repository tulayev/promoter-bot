using Deployf.Botf;
using Telegram.Bot;

namespace PromoterBot
{
    public class FilesController : BotController
    {
        private readonly IConfiguration _config;

        public FilesController(IConfiguration config)
        {
            _config = config;
        }

        [On(Handle.Unknown)]
        public async Task Unknown()
        {
            var document = Context.Update.Message?.Document;

            if (document is not null)
            {
                var file = await Context.Bot.Client.GetFileAsync(document!.FileId);
                string ext = System.IO.Path.GetExtension(file.FilePath!);
                string filePath = System.IO.Path.Combine(_config["FileStorage"], System.IO.Path.GetRandomFileName() + ext);
                await using var fs = new FileStream(filePath, FileMode.Create);
                await Context.Bot.Client.DownloadFileAsync(file.FilePath!, fs);

                PushL($"File {file.FilePath!} has been downloaded to {filePath}");
            }

            Context.StopHandling();
        }
    }
}
