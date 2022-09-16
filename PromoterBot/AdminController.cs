using AutoMapper;
using AutoMapper.QueryableExtensions;
using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Dtos;
using PromoterBot.Models;
using PromoterBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace PromoterBot
{
    public class AdminController : BotController
    {
        private readonly DataContext _ctx;
        
        private readonly IMapper _mapper;

        public AdminController(DataContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        [Action("Панел админа")]
        public async Task Start()
        {
            await Client.SendTextMessageAsync(
                chatId: Context.GetChatId(),
                text: "Выберите действие:",
                replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.AdminPanel)
            );

            await Import();
        }

        [Action]
        private async Task Import()
        {
            string command = await AwaitText();
            
            switch (command)
            {
                case "Скачать таблицу промоутеров":
                    AdminActions<Promoter>.ExportDataToExcel(await _ctx.Promoters.ToListAsync());
                    await DownloadFile();
                    break;
                case "Скачать таблицу участников":
                    var participants = await _ctx.Participants
                        .ProjectTo<ParticipantDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();
                    AdminActions<ParticipantDto>.ExportDataToExcel(participants);
                    await DownloadFile();
                    break;
                default:
                    await Send("Unknown command");
                    break;
            }

            await Start();
        }

        [Action]
        private async Task DownloadFile()
        {
            string path = "data.xlsx";

            if (File.Exists(path))
            {
                using var stream = File.OpenRead(path);
                await Context.Bot.Client.SendDocumentAsync(Context.GetSafeChatId(), new InputOnlineFile(stream, path));
            }    
        }
    }
}
