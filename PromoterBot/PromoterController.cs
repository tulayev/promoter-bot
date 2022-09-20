using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Dtos;
using PromoterBot.Models;
using PromoterBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PromoterBot
{
    public class PromoterController : BotController
    {
        private readonly ILogger<Program> _logger;

        private readonly DataContext _ctx;

        private readonly IConfiguration _config;

        private readonly Promoter _promoter = new();

        private const int PageSize = 6;

        public PromoterController(ILogger<Program> logger, DataContext ctx, IConfiguration config)
        {
            _logger = logger;
            _ctx = ctx;
            _config = config;
        }

        [Action("/start", "Начать!")]
        public async Task Start()
        {
            var promoters = await _ctx.Promoters.ToListAsync();

            if (_config["AdminId"] == Context.GetChatId().ToString())
            {
                PushL("Нажмите на кнопку, чтобы войти в панель админа");
                RowKButton(Q<AdminController>(c => c.Start));
            }
            else if (promoters.Any(p => p.ChatId == Context.GetChatId().ToString()))
            {
                PushL("Нажмите на кнопку, чтобы добавить участника!");
                RowKButton(Q<ParticipantController>(c => c.Add));
            }
            else
            {
                RequestContact();
            }
        }

        [Action]
        private void RequestContact()
        {
            PushL($"Пожалуйста, введите свой номер телефона.");
            RowKButton(KeyboardButton.WithRequestContact("Поделиться номером"));
        }

        [On(Handle.Unknown)]
        public async Task HandleContact()
        {
            var contact = Context.Update.Message.Contact;

            if (contact is null)
                return;

            await Send($"Ваш номер: {contact.PhoneNumber}");
            _promoter.PhoneNumber = contact.PhoneNumber;

            await EnterName();
            Context.StopHandling(); 
        }

        [Action]
        private async Task EnterName()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Пожалуйста, введите своё Ф.И.О.",
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );

            string input = await AwaitText();

            if (!String.IsNullOrEmpty(input))
            {
                if (input == Dictionaries.Commands["Prev"])
                {
                    await EnterName();
                }
                else if (input == Dictionaries.Commands["Cancel"] || input == Dictionaries.Commands["Start"])
                {
                    await Start();
                }
                else
                {
                    _promoter.Name = input;
                    _promoter.ChatId = Context.GetChatId().ToString();
                    await ChooseRegion();
                }
            }
        }

        [Action]
        private async Task ChooseRegion(int page = 1)
        {
            var source = _ctx.Regions.Include(r => r.Cities);
            int count = await source.CountAsync();
            var regions = await source.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestRegion"],
               replyMarkup: CustomKeyBoards.GetKeyboard(regions)
            );

            var pageDto = new PageDto(count, page, PageSize);

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
                await ChooseCity(chosenRegion);
            }
        }

        [Action]
        private async Task ChooseCity(Region region, int page = 1)
        {
            var source = region.Cities;
            int count = source.Count;
            var cities = source.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestCity"],
               replyMarkup: CustomKeyBoards.GetKeyboard(cities)
            );

            var pageDto = new PageDto(count, page, PageSize);

            string input = await AwaitText(() => _ = Send("Нажмите /start чтобы начать заново"));

            if (pageDto.HasPreviousPage && input == Dictionaries.Commands["Prev"])
            {
                await ChooseCity(region, --page);
            }
            else if (pageDto.HasNextPage && input == Dictionaries.Commands["Next"])
            {
                await ChooseCity(region, ++page);
            }
            else if ((!pageDto.HasPreviousPage && input == Dictionaries.Commands["Prev"]) ||
                (!pageDto.HasNextPage && input == Dictionaries.Commands["Next"]))
            {
                await ChooseCity(region, page);
            }
            else if (input == Dictionaries.Commands["Start"])
            {
                await Start();
            }
            else
            {
                _promoter.City = input;
                _ctx.Promoters.Add(_promoter);
                await _ctx.SaveChangesAsync();
                await Send("Регистрация прошла успешно!");
                await Start();
            }
        }

        [On(Handle.Exception)]
        public async Task Ex(Exception e)
        {
            _logger.LogCritical(e, "Unhandled exception");

            if (Context.Update.Type == UpdateType.CallbackQuery)
            {
                await AnswerCallback("Error");
            }
            else if (Context.Update.Type == UpdateType.Message)
            {
                Push("Error");
            }
        }

        [On(Handle.ChainTimeout)]
        public void ChainTimeout()
        {
            PushL("timeout");
        }
    }
}
