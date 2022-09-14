using Telegram.Bot.Types.ReplyMarkups;

namespace PromoterBot.Utils
{
    public static class CustomKeyBoards
    {
        public static ReplyKeyboardMarkup GetKeyboard(KeyBoardTypes types)
        {
            return types switch
            {
                KeyBoardTypes.Default => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Назад"),
                    new KeyboardButton("Вперёд")
                })
                {
                    ResizeKeyboard = true
                },
                _ => throw new Exception()
            };
        }
    }

    public enum KeyBoardTypes { Default }
}
