using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ConsoleAppNewsapi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string defaultlangSource = "все языки";
            string defaultApiKey = "27faf3aee9854015b5d934d8a475fdac";
            string defaultQuery = "космос";

            string query = ReadInput("Введите тему новостей", defaultQuery, false);
            Console.WriteLine($"Выбрана тема новостей: {query}");

            string apiKey = ReadApiKey("Введите apiKey", defaultApiKey);
            Console.WriteLine($"Выбран apiKey: {apiKey.Substring(0, 5)}...{apiKey.Substring(apiKey.Length - 3)}");

            string langSource = ReadInput("Выберите язык (ru, en)", defaultlangSource, false);
            Console.WriteLine($"Выбран язык новостей: {langSource}");
            langSource = (langSource == defaultlangSource) ? String.Empty : $"&language={langSource}";

            await TextProcessor(apiKey, query, langSource);

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey(); 
        }

        /// <summary>
        /// Производит работу с новостями
        /// </summary>
        /// <param name="apiKey">Ключ</param>
        /// <param name="query">Раздел</param>
        /// <param name="LangSource">Язык (может быть пустым)</param>
        /// <returns></returns>
        private static async Task TextProcessor(string apiKey, string query, string langSource)
        {
            //Получение новостей
            var newsList = await NewsApiResponse.GetNewsAsync(apiKey, query, langSource);
            if (newsList == null || newsList.Count() == 0)
            {
                Console.WriteLine($"Новости не найдены");
                return;
            }

            //Обработка каждой новости
            foreach (var news in newsList)
            {
                Console.WriteLine($"Новость: {news.Title}");
                Console.WriteLine($"Автор: {news.Author}");
                Console.WriteLine($"Описание: {news.Description.Trim()}");
                // Console.WriteLine($"Содержание: {news.Content}");

                //Получаем список слов с наибольшим количеством гласных
                HashSet<string> wordsWithMostVowels = NewsApiResponse.GetWordsWithMostVowels(news.Description, out int maxVowelCount); //Будем брать только уникальные

                if (wordsWithMostVowels == null || wordsWithMostVowels.Count() == 0)
                    continue;

                Console.WriteLine($"Слова с наибольшим количеством гласных ({maxVowelCount} гласных):");
                foreach (string word in wordsWithMostVowels)
                {
                    Console.WriteLine(word);
                }

                Console.WriteLine(new string('-', 100));
                await Task.Delay(500); //задержка просто так, для поэтапного вывода результата
            }

        }

        /// <summary>
        /// Обработка входных данных
        /// </summary>
        /// <param name="prompt">Сообщение</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="hide">Скрывать часть данных defaultValue</param>
        /// <returns>Входной параметр</returns>
        static string ReadInput(string prompt, string defaultValue, bool hide)
        {
            string defMess = hide ? $"{defaultValue.Substring(0, 5)}...{defaultValue.Substring(defaultValue.Length - 3)}" : defaultValue;
            Console.Write($"{prompt} (по умолчанию {defMess}): ");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                input = defaultValue;
            }
            return input;
        }

        /// <summary>
        ///  Обработка входных данных, с дополнительной проверкой
        /// </summary>
        /// <param name="prompt">Сообщение</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Входной параметр</returns>
        static string ReadApiKey(string prompt, string defaultValue)
        {
            string input = ReadInput(prompt, defaultValue, true);

            if (input.Length < 7)
            {
                Console.WriteLine("Некорректный apiKey, значение apiKey будет использовано по умолчанию.");
                input = defaultValue;
            }

            return input;
        }


    }
}
