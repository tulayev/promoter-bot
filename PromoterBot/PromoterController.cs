using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Models;
using Telegram.Bot.Types.Enums;

namespace PromoterBot
{
    public class PromoterController : BotController
    {
        private readonly ILogger<Program> _logger;

        private readonly DataContext _ctx;

        private readonly Promoter _promoter = new();

        public PromoterController(ILogger<Program> logger, DataContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [Action("/start", "Начать!")]
        public async Task Start()
        {
            var promoters = await _ctx.Promoters.ToListAsync();

            if (promoters.Any(p => p.ChatId == Context.GetChatId().ToString()))
            {
                PushL("Нажмите на кнопку, чтобы добавить участника!");
                RowKButton(Q<ParticipantController>(c => c.Add));
            }
            else
            {
                await EnterName();
            }
        }

        [Action]
        private async Task EnterName()
        {
            _promoter.ChatId = Context.GetChatId().ToString();

            await Send($"Пожалуйста, введите своё имя.");

            string name = await AwaitText();

            Button("Назад");
            Button("Вперёд");
            await Send($"Ваше имя: {name}!");
            string btn = await AwaitQuery();
            _promoter.Name = name;

            if (btn == "Назад")
                await EnterName();
            else
                await EnterPhone();
        }

        [Action]
        private async Task EnterPhone()
        {
            await Send($"Пожалуйста, введите свой номер телефона.");
            string phone = await AwaitText();

            Button("Назад");
            Button("Вперёд");
            await Send($"Ваш номер: {phone}");
            string btn = await AwaitQuery();
            _promoter.PhoneNumber = Int32.Parse(phone);

            if (btn == "Назад")
                await EnterPhone();
            else
                await EnterCity();
        }
        
        [Action]
        private async Task EnterCity()
        {
            await Send($"Пожалуйста, введите свой город.");
            string city = await AwaitText();

            Button("Назад");
            Button("Вперёд");
            await Send($"Ваш город: {city}");
            string btn = await AwaitQuery();
            _promoter.City = city;

            if (btn == "Назад")
                await EnterCity();
            else
                await Save();
        }

        [Action]
        private async Task Save()
        {
            _ctx.Promoters.Add(_promoter);
            await _ctx.SaveChangesAsync();
            await Send("Регистрация прошла успешно!");
        }

        [On(Handle.Unknown)]
        public async Task Unknown()
        {
            PushL("unknown");
            await Send();
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
