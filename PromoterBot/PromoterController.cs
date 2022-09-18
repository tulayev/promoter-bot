using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Models;
using PromoterBot.Utils;
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
            await Send($"Пожалуйста, введите своё Ф.И.О.");

            string name = await AwaitText();
            _promoter.Name = name;
            _promoter.ChatId = Context.GetChatId().ToString();

            Button(Dictionaries.Commands["Prev"]);
            Button(Dictionaries.Commands["Next"]);
            await Send($"Ваше Ф.И.О.: {name}!");
            string btn = await AwaitQuery();


            if (btn == Dictionaries.Commands["Prev"])
            {
                await EnterName();
            }
            else
            {
                await EnterCity();
            }
        }

        [Action]
        private async Task EnterCity()
        {
            await Send($"Пожалуйста, введите свой город.");
            string city = await AwaitText();

            _promoter.City = city;

            Button(Dictionaries.Commands["Prev"]);
            Button(Dictionaries.Commands["Next"]);
            await Send($"Ваш город: {city}");
            string btn = await AwaitQuery();
            _promoter.City = city;

            if (btn == Dictionaries.Commands["Prev"])
            {
                await EnterCity();
            }
            else
            {
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
