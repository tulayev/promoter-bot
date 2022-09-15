using Telegram.Bot.Types.ReplyMarkups;

namespace PromoterBot.Utils
{
    public static class CustomKeyBoards
    {
        public static ReplyKeyboardMarkup GetKeyboard(KeyBoardTypes types)
        {
            return types switch
            {
                KeyBoardTypes.GenderSelect => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Мужской"),
                    new KeyboardButton("Женский")
                })
                {
                    ResizeKeyboard = true
                },
                KeyBoardTypes.SocilaNetWorkSelect => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Telegram"),
                    new KeyboardButton("Facebook"),
                    new KeyboardButton("Instagram"),
                    new KeyboardButton("Youtube"),
                    new KeyboardButton("Другой")
                })
                {
                    ResizeKeyboard = true
                },
                _ => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Отмена")
                })
                {
                    ResizeKeyboard = true
                },
            };
        }
    }

    public enum KeyBoardTypes { Default, GenderSelect, SocilaNetWorkSelect }
}
