namespace PromoterBot.Utils
{
    public static class Dictionaries
    {
        public static Dictionary<string, string> Commands => new()
        {
            ["Prev"] = "Назад",
            ["Next"] = "Вперёд",
            ["Other"] = "Другой",
            ["Cancel"] = "Отмена",
            ["Start"] = "/start",
            ["DownloadPromoters"] = "Скачать таблицу промоутеров",
            ["DownloadParticipants"] = "Скачать таблицу участников",
            ["AddRegion"] = "Добавить регион",
            ["AddCity"] = "Добавить город"
        };

        public static Dictionary<string, string> Requests => new()
        {
            ["RequestPhoto"] = "Пожалуйста, отправьте фото участника файлом, а не картинкой!",
            ["RequestName"] = "Пожалуйста, введите Ф.И.О. участника",
            ["RequestPhoneNumber"] = "Пожалуйста, введите номер телефона участника в формате +998XXXXXXXXX",
            ["RequestAge"] = "Пожалуйста, введите возраст участника",
            ["RequestGender"] = "Выберите пол участника",
            ["RequestRegion"] = "Пожалуйста, выберите район",
            ["RequestCity"] = "Пожалуйста, выберите город",
            ["RequestSocialNetwork"] = "Выберите предпочитаемую соц. сети участника",
            ["RequestBrands"] = "Пожалуйста, введите любимые бренды участника"
        };
    }
}
