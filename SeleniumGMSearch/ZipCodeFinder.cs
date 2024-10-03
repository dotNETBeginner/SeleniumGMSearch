using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeleniumGMSearch
{
    public class ZipCodeFinder : IZipCodeFinder
    {
        private readonly Dictionary<string, string> ZipCodesMapping;

        public ZipCodeFinder()
        {
            ZipCodesMapping = new Dictionary<string, string>()
            { { "USA", @"\b(?:(\d{5})(?:-(\d{4}))?)\b" }, // США
                { "Canada", @"\b(?:(A[0-9][A-Z] ?[0-9][A-Z][0-9]))\b" }, // Канада
                { "UK", @"\b(?:(?:[A-Z]{1,2}[0-9][0-9]?(?:[A-Z]?)?\s?[0-9][A-Z]{2})|(?:[A-Z][0-9][A-Z]?\s?[0-9][A-Z]{2})|(?:[A-Z]{1,2}[0-9][A-Z]?\s?[0-9][A-Z]{2})|(?:[A-Z]{1,2}[0-9]{1,2}\s?[0-9][A-Z]{2}))\b" }, // Великобритания
                { "Germany", @"\b(?:(\d{5}))\b" }, // Германия
                { "France", @"\b(?:(\d{5}))\b" }, // Франция
                { "Italy", @"\b(?:(\d{5}))\b" }, // Италия
                { "Australia", @"\b(?:(\d{4}))\b" }, // Австралия
                { "Sweden", @"\b(?:(\d{3}) ?(\d{2}))\b" }, // Швеция
                { "Netherlands", @"\b(?:(\d{4}) ?([A-Za-z]{2}))\b" }, // Нидерланды

                { "Spain", @"\b(?:(\d{5}))\b" }, // Испания
                { "Brazil", @"\b(?:(\d{5}-\d{3}))\b" }, // Бразилия
                { "Japan", @"\b(?:(\d{3}-\d{4}))\b" }, // Япония
                { "New Zealand", @"\b(?:(\d{4}))\b" }, // Новая Зеландия
                { "Mexico", @"\b(?:(\d{5}))\b" }, // Мексика
                { "India", @"\b(?:(\d{6}))\b" }, // Индия
                { "Argentina", @"\b(?:(\d{4}) ?([A-Za-z]{3}))\b" }, // Аргентина
                { "South Korea", @"\b(?:(\d{5}))\b" }, // Южная Корея
                { "Russia", @"\b(?:(\d{6}))\b" }, // Россия
                { "Denmark", @"\b(?:(\d{4}))\b" }, // Дания

                { "Finland", @"\b(?:(\d{5}))\b" }, // Финляндия
                { "Norway", @"\b(?:(\d{4}))\b" }, // Норвегия
                { "Poland", @"\b(?:(\d{2}-\d{3}))\b" }, // Польша
                { "Czech Republic", @"\b(?:(\d{3}) ?(\d{2}))\b" }, // Чехия
                { "Slovakia", @"\b(?:(\d{5}))\b" }, // Словакия
                { "Austria", @"\b(?:(\d{4}))\b" }, // Австрия
                { "Switzerland", @"\b(?:(\d{4}))\b" }, // Швейцария
                { "Portugal", @"\b(?:(\d{4}-\d{3}))\b" }, // Португалия
                { "Turkey", @"\b(?:(\d{5}))\b" }, // Турция
                { "Ireland", @"\b(?:(\d{1,2} ?[A-Za-z]{1,2}\d{1,2}))\b" }, // Ирландия

                { "Greece", @"\b(?:(\d{5}))\b" }, // Греция
                { "Slovenia", @"\b(?:(\d{4}))\b" }, // Словения
                { "Latvia", @"\b(?:(\d{4}))\b" }, // Латвия
                { "Lithuania", @"\b(?:(\d{5}))\b" }, // Литва
                { "Estonia", @"\b(?:(\d{5}))\b" }, // Эстония
                { "Croatia", @"\b(?:(\d{5}))\b" }, // Хорватия
                { "Serbia", @"\b(?:(\d{5}))\b" }, // Сербия
                { "Bulgaria", @"\b(?:(\d{4}))\b" }, // Болгария
                { "Romania", @"\b(?:(\d{6}))\b" }, // Румыния
                { "Cyprus", @"\b(?:(\d{4}))\b" }, // Кипр

                { "Malta", @"\b(?:(A[0-9]{3}) ?(\d{4}))\b" }, // Мальта
                { "Maldives", @"\b(?:(\d{5}))\b" }, // Мальдивы
                { "Kazakhstan", @"\b(?:(\d{6}))\b" }, // Казахстан
                { "Uzbekistan", @"\b(?:(\d{6}))\b" }, // Узбекистан
                { "Kyrgyzstan", @"\b(?:(\d{6}))\b" }, // Кыргызстан
                { "Tajikistan", @"\b(?:(\d{6}))\b" }, // Таджикистан
                { "Azerbaijan", @"\b(?:(\d{4}))\b" }, // Азербайджан
                { "Georgia", @"\b(?:(\d{4}))\b" }, // Грузия
                { "Armenia", @"\b(?:(\d{4}))\b" }, // Армения
                { "Turkmenistan", @"\b(?:(\d{6}))\b" } // Туркменистан};

            };
        }

        public string FindZipCode(string address)
        {
            foreach(var item in ZipCodesMapping)
            {
                MatchCollection matches = Regex.Matches(address, item.Value);
                if (matches.Count > 0)
                {
                    return matches[0].Value;
                }
            }

            return "";
        }
    }
}
