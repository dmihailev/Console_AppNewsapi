using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace ConsoleAppNewsapi
{
    public class NewsApiResponse
    {
        public List<Article> Articles { get; set; }

        /// <summary>
        /// Получение новостей из источника данных
        /// </summary>
        /// <param name="apiKey">Ключ</param>
        /// <param name="query">Раздел</param>
        /// <param name="LangSource">Язык (может быть пустым)</param>
        /// <returns>Список новостей</returns>
        public static async Task<IEnumerable<Article>> GetNewsAsync(string apiKey, string query, string LangSource)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "ConsoleAppNewsapi/1.0");
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                string apiUrl = $"https://newsapi.org/v2/everything?q={query}&from=2023-09-10&sortBy=publishedAt{LangSource}&apiKey={apiKey}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var news = JsonConvert.DeserializeObject<NewsApiResponse>(json);
                    return news.Articles.Select(article => new Article
                    {
                        Author = article.Author,
                        Title = article.Title,
                        Description = article.Description,
                        Content = article.Content
                    });
                }
                else
                {
                    int statusCodeValue = (int)response.StatusCode;
                    Console.WriteLine($"Ошибка при получении новостей: {response.ReasonPhrase}: {statusCodeValue}");
                    return null;
                }
            }
        }

        /// <summary>
        /// Получение списка слов с наибольшим количеством гласных
        /// </summary>
        /// <param name="description">Исходный текст</param>
        /// <param name="maxVowelCountValue">Максимальное количество гласных в слове(ах)</param>
        /// <returns>Возвращает список слов и количество гласных</returns>
        public static HashSet<string> GetWordsWithMostVowels(string description, out int maxVowelCountValue)
        {
            maxVowelCountValue = 0;
            // Разбиваем текст новости на слова и находим слово с наибольшим количеством гласных
            string[] separators = new string[] { " ", "  ", "\u00A0", ".", ",", ";", ":", "\"", "\'", "!", "?", "=", "\n", "\r" };


            if (string.IsNullOrWhiteSpace(description))
                return null;

            string[] words = description.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> wordsWithMostVowels = new HashSet<string>(); //Будем брать только уникальные

            int maxVowelCount = 0;

            foreach (string word in words)
            {
                int vowelCount = CountVowels(word);

                if (vowelCount > maxVowelCount)
                {
                    maxVowelCount = vowelCount;
                    wordsWithMostVowels.Clear(); // Очищаем список, так как нашли новое максимальное значение
                    wordsWithMostVowels.Add(word);
                }
                else if (vowelCount == maxVowelCount)
                {
                    wordsWithMostVowels.Add(word); // Добавляем слово с таким же количеством гласных
                }
            }
            maxVowelCountValue = maxVowelCount;

            return wordsWithMostVowels;
        }

        /// <summary>
        /// Подсчет количества гласных
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns>Количество гласных</returns>
        public static int CountVowels(string text)
        {
            string vowels = "AEIOUaeiouАЕЁИОУЭЮЯаеёиоуэюя";
            return text.Count(c => vowels.Contains(c));
        }
    }
    
    public class Article
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
