using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Models;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace PromoterBot
{
    public class ParticipantController : BotController
    {
        private readonly ILogger<Program> _logger;

        private readonly DataContext _ctx;

        private readonly Participant _participant = new();

        public ParticipantController(ILogger<Program> logger, DataContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [Action("Добавить участника")]
        public async Task Add()
        {
            await EnterName();
        }

        [Action]
        private async Task EnterName()
        {
            await Send($"Пожалуйста, введите Ф.И.О. участника");

            string input = await AwaitText();
            
            _participant.Name = input;

            Button("Назад");
            Button("Вперёд");
            await Send($"Ф.И.О. участника: {input}!");
            string btn = await AwaitQuery();


            if (btn == "Назад")
                await EnterName();
            else
                await EnterPhoneNumber();
        }

        [Action]
        private async Task EnterPhoneNumber()
        {
            await Send($"Пожалуйста, введите номер телефона участника в формате +998XXXXXXXXX");
            string input = await AwaitText();

            string regex = @"(?:\+[9]{2}[8][0-9]{2}[0-9]{3}[0-9]{2}[0-9]{2})";
            var match = Regex.Match(input, regex, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                await Send("Номер телефона имел неверный формат.");
                await EnterPhoneNumber();
            }

            _participant.PhoneNumber = input;

            Button("Назад");
            Button("Вперёд");
            await Send($"Номер телефона участника: {input}");
            string btn = await AwaitQuery();

            if (btn == "Назад")
                await EnterPhoneNumber();
            else
                await EnterAge();
        }

        [Action]
        private async Task EnterAge()
        {
            await Send($"Пожалуйста, введите возраст участника");

            string input = await AwaitText();
            int age = 0;

            if (Int32.TryParse(input, out int res))
            {
                age = res;
            }
            else
            {
                await Send($"Пожалуйста, введите корректное число");
                await EnterAge();
            }

            _participant.Age = age;

            Button("Назад");
            Button("Вперёд");
            await Send($"Возраст участника: {age}!");
            string btn = await AwaitQuery();


            if (btn == "Назад")
                await EnterAge();
            else
                await EnterGender();
        }

        [Action]
        private async Task EnterGender()
        {
            // temp
            await Save();
        }

        private async Task Save()
        {
            var promoter = await _ctx.Promoters
                    .FirstOrDefaultAsync(p => p.ChatId == Context.GetChatId().ToString());

            _participant.PromoterId = promoter.Id;

            _ctx.Participants.Add(_participant);
            await _ctx.SaveChangesAsync();
            await Send("Участник успешно добавлен!");
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
