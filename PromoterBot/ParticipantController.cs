using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using PromoterBot.Data;
using PromoterBot.Models;
using PromoterBot.Utils;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PromoterBot
{
    public class ParticipantController : BotController
    {
        private readonly ILogger<Program> _logger;

        private readonly IWebHostEnvironment _env;

        private readonly DataContext _ctx;
        
        private readonly Participant _participant = new();

        private readonly List<string> _socialNetworks = new();

        public ParticipantController(ILogger<Program> logger, IWebHostEnvironment env, DataContext ctx)
        {
            _logger = logger;
            _env = env;
            _ctx = ctx;
        }

        [Action("Добавить участника")]
        public async Task Add()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestPhoto"],
               replyMarkup: new ReplyKeyboardRemove()
            );
        }

        [Action]
        private async Task EnterName()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestName"],
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );

            string input = await AwaitText();

            if (!String.IsNullOrEmpty(input))
            {
                if (input == Dictionaries.Commands["Prev"] || input == Dictionaries.Commands["Cancel"])
                {
                    await Add();
                }
                else
                {
                    _participant.Name = input;
                    await EnterPhoneNumber();
                }
            }
        }

        [Action]
        private async Task EnterPhoneNumber()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestPhoneNumber"],
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );

            string input = await AwaitText();

            string regex = @"(?:\+[9]{2}[8][0-9]{2}[0-9]{3}[0-9]{2}[0-9]{2})";
            var match = Regex.Match(input, regex, RegexOptions.IgnoreCase);

            if (!String.IsNullOrEmpty(input))
            {
                if (input == Dictionaries.Commands["Prev"])
                {
                    await EnterName();
                }
                else if (input == Dictionaries.Commands["Cancel"])
                {
                    await Add();
                }
                else if (!match.Success)
                {
                    await EnterPhoneNumber();
                }
                else
                {
                    _participant.PhoneNumber = input;
                    await EnterAge();
                }
            }
        }

        [Action]
        private async Task EnterAge()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestAge"],
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );
            
            string input = await AwaitText();

            bool isValid = Int32.TryParse(input, out int res);

            if (!String.IsNullOrEmpty(input))
            {
                if (input == Dictionaries.Commands["Prev"])
                {
                    await EnterPhoneNumber();
                }
                else if (input == Dictionaries.Commands["Cancel"])
                {
                    await Add();
                }
                else if (!isValid)
                {
                    await EnterAge();
                }
                else
                {
                    _participant.Age = res;
                    await EnterGender();
                }
            }
        }

        [Action]
        private async Task EnterGender()
        {
            await Client.SendTextMessageAsync(
                chatId: Context.GetSafeChatId(),
                text: Dictionaries.Requests["RequestGender"],
                replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.GenderSelect)
            );

            string input = await AwaitText();

            if (!String.IsNullOrEmpty(input))
            {
                if (input == Dictionaries.Commands["Prev"])
                {
                    await EnterAge();
                }
                else if (input == Dictionaries.Commands["Cancel"])
                {
                    await Add();
                }
                else if (input != "Женский" && input != "Мужской")
                {
                    await EnterGender();
                }
                else
                {
                    _participant.Gender = input;
                    await EnterSocialNetwork();
                }
            }
        }

        [Action]
        private async Task EnterSocialNetwork()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestSocialNetwork"],
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.SocilaNetWorkSelect)
            );
            
            string input = await AwaitText();

            if (input == Dictionaries.Commands["Other"])
            {
                _socialNetworks.Add(await GetManualSocialInput());
            }

            if (!String.IsNullOrEmpty(input))
            {
                if (input == Dictionaries.Commands["Prev"])
                {
                    await EnterGender();
                }
                else if (input == Dictionaries.Commands["Cancel"])
                {
                    await Add();
                }
                else if (input == Dictionaries.Commands["Next"])
                {
                    await EnterFavouriteBrands();
                }
                else
                {
                    _socialNetworks.Add(input);

                    if (_socialNetworks.Contains(Dictionaries.Commands["Other"]))
                    {
                        _socialNetworks.Remove(Dictionaries.Commands["Other"]);
                    }

                    _participant.SocialNetwork = String.Join(',', _socialNetworks);
                    await EnterSocialNetwork();
                }
            }
        }

        private async Task<string> GetManualSocialInput()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: "Введите вручную соц. сеть!",
               replyMarkup: new ReplyKeyboardRemove()
            );

            string input = await AwaitText();

            return input;
        }

        [Action]
        private async Task EnterFavouriteBrands()
        {
            await Client.SendTextMessageAsync(
               chatId: Context.GetSafeChatId(),
               text: Dictionaries.Requests["RequestBrands"],
               replyMarkup: CustomKeyBoards.GetKeyboard(KeyBoardTypes.Default)
            );

            string input = await AwaitText();

            if (!String.IsNullOrEmpty(input))
            {
                if (input == Dictionaries.Commands["Prev"])
                {
                    await EnterSocialNetwork();
                }
                else if (input == Dictionaries.Commands["Cancel"])
                {
                    await Add();
                }
                else
                {
                    _participant.FavouriteBrands = input;
                    await Save();
                }
            }
        }

        private async Task Save()
        {
            var promoter = await _ctx.Promoters
                    .FirstOrDefaultAsync(p => p.ChatId == Context.GetChatId().ToString());

            _participant.City = promoter.City;
            _participant.PromoterId = promoter.Id;

            _ctx.Participants.Add(_participant);
            await _ctx.SaveChangesAsync();
            await Send("Участник успешно добавлен!");
            PushL("Нажмите /start, чтобы добавить нового участника!");
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
