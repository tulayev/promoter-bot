using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Dtos;
using PromoterBot.Models;
using PromoterBot.Utils;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PromoterBot
{
    public class ParticipantController : BotController
    {
        private readonly ILogger<Program> _logger;

        private readonly IWebHostEnvironment _env;

        private readonly DataContext _ctx;

        private readonly Participant _participant = new();

        private const int PageSize = 6;

        public ParticipantController(ILogger<Program> logger, IWebHostEnvironment env, DataContext ctx)
        {
            _logger = logger;
            _env = env;
            _ctx = ctx;
        }

        [Action("Добавить участника")]
        public async Task Add()
        {
            await Send("Пожалуйста, отправьте фото участника");
        }

        [Action]
        private async Task EnterName()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Пожалуйста, введите Ф.И.О. участника",
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );

            string input = await AwaitText();

            if (Canceled(input))
            {
                await Add();
                return;
            }
            
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
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Пожалуйста, введите номер телефона участника в формате +998XXXXXXXXX",
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );

            string input = await AwaitText();

            if (Canceled(input))
            {
                await Add();
                return;
            }

            string regex = @"(?:\+[9]{2}[8][0-9]{2}[0-9]{3}[0-9]{2}[0-9]{2})";
            var match = Regex.Match(input, regex, RegexOptions.IgnoreCase);

            _participant.PhoneNumber = input;

            Button("Назад");
            Button("Вперёд");
            await Send($"Номер телефона участника: {input}");
            string btn = await AwaitQuery();

            if (btn == "Назад" || !match.Success)
                await EnterPhoneNumber();
            else
                await EnterAge();
        }

        [Action]
        private async Task EnterAge()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Пожалуйста, введите возраст участника",
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );
            
            string input = await AwaitText();

            if (Canceled(input))
            {
                await Add();
                return;
            }

            bool isValid = Int32.TryParse(input, out int res);

            Button("Назад");
            Button("Вперёд");
            await Send($"Возраст участника: {input}!");
            string btn = await AwaitQuery();

            if (btn == "Назад" || !isValid)
                await EnterAge();
            else
            {
                _participant.Age = res;
                await EnterGender();
            }
        }

        [Action]
        private async Task EnterGender()
        {
            await Client.SendTextMessageAsync(
                chatId: Context.GetSafeChatId(),
                text: "Выберите пол участника",
                replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.GenderSelect)
            );

            string input = await AwaitText();

            _participant.Gender = input;

            Button("Назад");
            Button("Вперёд");
            await Send($"Пол участника: {input}!");
            string btn = await AwaitQuery();

            if (btn == "Назад")
                await EnterGender();
            else
                await ChooseRegion();
        }

        [Action]
        private async Task ChooseRegion(int page = 1)
        {
            var source = _ctx.Regions.Include(r => r.Cities);
            int count = await source.CountAsync();
            var regions = await source.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Пожалуйста, выберите район",
               replyMarkup: CustomKeyBoards.GetKeyboard(regions)
            );

            var pageDto = new PageDto(count, page, PageSize);

            string input = await AwaitText();

            if (pageDto.HasPreviousPage && input == "Назад")
            {
                await ChooseRegion(--page);
            }
            else if (pageDto.HasNextPage && input == "Вперёд")
            {
                await ChooseRegion(++page);
            }
            else if ((!pageDto.HasPreviousPage && input == "Назад") || (!pageDto.HasNextPage && input == "Вперёд"))
            {
                await ChooseRegion(page);
            }
            
            var chosenRegion = regions.FirstOrDefault(r => r.Name == input);

            await ChooseCity(chosenRegion);
        }

        [Action]
        private async Task ChooseCity(Region region, int page = 1)
        {
            var source = region.Cities;
            int count = source.Count;
            var cities = source.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Пожалуйста, выберите город",
               replyMarkup: CustomKeyBoards.GetKeyboard(cities)
            );

            var pageDto = new PageDto(count, page, PageSize);

            string input = await AwaitText();

            if (pageDto.HasPreviousPage && input == "Назад")
            {
                await ChooseCity(region, --page);
            }
            else if (pageDto.HasNextPage && input == "Вперёд")
            {
                await ChooseCity(region, ++page);
            }
            else if ((!pageDto.HasPreviousPage && input == "Назад") || (!pageDto.HasNextPage && input == "Вперёд"))
            {
                await ChooseCity(region, page);
            }

            _participant.City = input;
            
            await EnterSocialNetwork();
        }

        [Action]
        private async Task EnterSocialNetwork()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Выберите прелпочитаемую соц. сети участника",
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.SocilaNetWorkSelect)
            );

            string input = await AwaitText();

            if (input == "Другой")
            {
                await Send("Введите вручную соц. сеть:");
                input = await AwaitText();
            }

            _participant.SocialNetwork = input;

            Button("Назад");
            Button("Вперёд");
            await Send($"Предпочитаемая соц. сети участника: {input}!");
            string btn = await AwaitQuery();

            if (btn == "Назад")
                await EnterSocialNetwork();
            else
                await EnterFavouriteBrands();
        }

        [Action]
        private async Task EnterFavouriteBrands()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Пожалуйста, введите любимые бренды участника",
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );

            string input = await AwaitText();

            if (Canceled(input))
            {
                await Add();
                return;
            }

            _participant.FavouriteBrands = input;

            Button("Назад");
            Button("Вперёд");
            await Send($"Любимые бренды участника: {input}!");
            string btn = await AwaitQuery();

            if (btn == "Назад")
                await EnterFavouriteBrands();
            else
                await Save();
        }

        private bool Canceled(string command) => command == "Отмена";

        private async Task Save()
        {
            var promoter = await _ctx.Promoters
                    .FirstOrDefaultAsync(p => p.ChatId == Context.GetChatId().ToString());

            _participant.PromoterId = promoter.Id;

            _ctx.Participants.Add(_participant);
            await _ctx.SaveChangesAsync();
            await Send("Участник успешно добавлен!");
            PushL("Нажмите /start, чтобы добавить нового участника!");
            return;
        }

        [On(Handle.Unknown)]
        public async Task HandleUpload()
        {   
            var document = Context.Update.Message?.Document;

            if (document is null)
                return;

            _participant.Image = await GetUploadedImagePath(document);

            await EnterName();

            Context.StopHandling();
        }

        private async Task<string> GetUploadedImagePath(Document document)
        {
            var file = await Context.Bot.Client.GetFileAsync(document!.FileId);
            string ext = System.IO.Path.GetExtension(file.FilePath!);
            string fileName = System.IO.Path.GetRandomFileName() + ext;
            string filePath = System.IO.Path.Combine(_env.WebRootPath, fileName);
            await using var fs = new FileStream(filePath, FileMode.Create);
            await Context.Bot.Client.DownloadFileAsync(file.FilePath!, fs);

            return fileName;
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
