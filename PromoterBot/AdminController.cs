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
using Telegram.Bot.Types.ReplyMarkups;

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

            await HandleCommands();
        }

        [Action]
        private async Task HandleCommands()
        {
            string command = await AwaitText();
            
            switch (command)
            {
                case "Скачать таблицу промоутеров":
                    var promoters = await _ctx.Promoters.ToListAsync();

                    if (promoters.Count > 0)
                    {
                        AdminActions.ExportDataToExcel(promoters);
                        await DownloadFile("data.xlsx");
                    }
                    else
                    {
                        await Send("Нет зарегестрированных промоутеров");
                    }
                    break;
                case "Скачать таблицу участников":
                    var participants = await _ctx.Participants
                        .ProjectTo<ParticipantDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    if (participants.Count > 0)
                    {
                        AdminActions.ExportDataToExcel(participants);
                        await DownloadFile("data.xlsx");
                    }
                    else
                    {
                        await Send("Нет зарегестрированных участников");
                    }
                    break;
                case "Добавить регион":
                    await AddRegion();
                    break;
                case "Добавить город":
                    await ChooseRegion();
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
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Введите название региона:",
               replyMarkup: new ReplyKeyboardRemove()
            );

            string input = await AwaitText();

            if (String.IsNullOrEmpty(input)) 
            {
                await AddRegion();
            }
            else if (input == Dictionaries.Commands["Start"])
            {
                await Start();
            }
            else
            {
                _ctx.Regions.Add(new Region { Name = input });
                await _ctx.SaveChangesAsync();
                await Send("Регион добавлен!");
                await Start();
            }
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
            else if (input == Dictionaries.Commands["Start"])
            {
                await Start();
            }
            else
            {
                var chosenRegion = regions.FirstOrDefault(r => r.Name == input);
                await AddCity(chosenRegion.Id);
            }
        }

        private async Task AddCity(int regionId)
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Введите название города:",
               replyMarkup: new ReplyKeyboardRemove()
            );
            
            string input = await AwaitText();

            if (String.IsNullOrEmpty(input))
            {
                await AddCity(regionId);
            }
            else if (input == Dictionaries.Commands["Start"])
            {
                await Start();
            }
            else
            {
                _ctx.Cities.Add(new City{ Name = input, RegionId = regionId });
                await _ctx.SaveChangesAsync();
                await Send("Город добавлен!");
                await Start();
            }
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
