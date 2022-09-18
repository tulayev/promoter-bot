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

        private readonly IWebHostEnvironment _env;

        public AdminController(DataContext ctx, IMapper mapper, IWebHostEnvironment env)
        {
            _ctx = ctx;
            _mapper = mapper;
            _env = env; 
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
                    AdminActions.ExportDataToExcel(await _ctx.Promoters.ToListAsync());
                    await DownloadFile("data.xlsx");
                    break;
                case "Скачать таблицу участников":
                    var participants = await _ctx.Participants
                        .ProjectTo<ParticipantDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();
                    AdminActions.ExportDataToExcel(participants);
                    await DownloadFile("data.xlsx");
                    break;
                case "Добавить регион":
                    await AddRegion();
                    break;
                case "Добавить город":
                    await ChooseRegion();
                    break;
                case "Скачать фотографию участника":
                    await Send("Введите название фотографии участника. Например: example.jpg");
                    string input = await AwaitText();
                    await DownloadFile(System.IO.Path.Combine(_env.WebRootPath, input));
                    break;
                default:
                    await Send("Unknown command");
                    break;
            }

            await Start();
        }

        [Action]
        private async Task AddRegion()
        {
            await Send("Введите название региона:");
            string input = await AwaitText();

            if (String.IsNullOrEmpty(input)) 
            {
                await AddRegion();
            }

            _ctx.Regions.Add(new Region { Name = input });
            await _ctx.SaveChangesAsync();
            await Send("Регион добавлен!");
            await Start();
        }

        [Action]
        private async Task ChooseRegion(int page = 1)
        {
            var source = _ctx.Regions.Include(r => r.Cities);
            int count = await source.CountAsync();
            var regions = await source.Skip((page - 1) * 6).Take(6).ToListAsync();

            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestRegion"],
               replyMarkup: CustomKeyBoards.GetKeyboard(regions)
            );

            var pageDto = new PageDto(count, page, 6);

            string input = await AwaitText();

            if (pageDto.HasPreviousPage && input == Dictionaries.Commands["Prev"])
            {
                await ChooseRegion(--page);
            }
            else if (pageDto.HasNextPage && input == Dictionaries.Commands["Next"])
            {
                await ChooseRegion(++page);
            }
            else if ((!pageDto.HasPreviousPage && input == Dictionaries.Commands["Prev"]) ||
                (!pageDto.HasNextPage && input == Dictionaries.Commands["Next"]))
            {
                await ChooseRegion(page);
            }

            var chosenRegion = regions.FirstOrDefault(r => r.Name == input);

            await AddCity(chosenRegion.Id);
        }

        private async Task AddCity(int regionId)
        {
            await Send("Введите название города:");
            string input = await AwaitText();

            if (String.IsNullOrEmpty(input))
            {
                await AddCity(regionId);
            }

            _ctx.Cities.Add(new City{ Name = input, RegionId = regionId });
            await _ctx.SaveChangesAsync();
            await Send("Город добавлен!");
            await Start();
        }

        [Action]
        private async Task DownloadFile(string path)
        {
            if (File.Exists(path))
            {
                using var stream = File.OpenRead(path);
                await Context.Bot.Client.SendDocumentAsync(Context.GetSafeChatId(), new InputOnlineFile(stream, path));
            }
            else
            {
                await Send("Файл не найден");
            }
        }
    }
}
