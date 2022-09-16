using PromoterBot.Models;
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
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Telegram"),
                        new KeyboardButton("Facebook"),
                        new KeyboardButton("Instagram")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Youtube"),
                        new KeyboardButton("Другой")
                    }
                })
                {
                    ResizeKeyboard = true
                },
                KeyBoardTypes.AdminPanel => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Скачать таблицу промоутеров"),
                    new KeyboardButton("Скачать таблицу участников")
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

        public static ReplyKeyboardMarkup GetKeyboard<T>(List<T> source) where T : Location
        {
            return new ReplyKeyboardMarkup(new[]
            {
                source.Take(3).Select(x => new KeyboardButton(x.Name)),
                source.Skip(3).Select(x => new KeyboardButton(x.Name)),
                new KeyboardButton[]
                {
                    new KeyboardButton("Назад"),
                    new KeyboardButton("Вперёд")
                }
            })
            {
                ResizeKeyboard = true
            };
        }
    }

    public enum KeyBoardTypes { Default, GenderSelect, SocilaNetWorkSelect, AdminPanel }
}
