using Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Markdig;
namespace Chat.Controllers
{
    public class GrammarController : Controller
    {
        private readonly IMemoryCache _cache;

        public GrammarController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public IActionResult Index()
        {
            // При каждом обращении к контроллеру удаляем старый объект UserChats из кеша и создаем новый
            _cache.Remove("UserChats" + GetOrSetUserIdFromCookie());

            GetOrSetUserChatsFromCache(GetOrSetUserIdFromCookie());

            return View();
        }

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> SendMessage([FromBody] MessageModel messageModel)
        {
            string message = messageModel.message;

            var userChats = GetOrSetUserChatsFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await userChats.OpenAIService.GetResponseAsync(message));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        public async Task<ActionResult> GetRules([FromBody] MessageModel messageModel)
        {
            var userChats = GetOrSetUserChatsFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await userChats.OpenAIService.GetResponseAsync("What rules did you apply to the corrected text. If the text was not provided for verification or it is not in English, then write about it to the user"));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        public async Task<ActionResult> GetExamples([FromBody] MessageModel messageModel)
        {
            var userChats = GetOrSetUserChatsFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await userChats.OpenAIService.GetResponseAsync("Give a few sentences using the rules you applied"));

            // Return the response to the client
            return Content(response);
        }

        private UserChats GetOrSetUserChatsFromCache(string userId)
        {
            return _cache.GetOrCreate("UserChats" + userId, entry =>
            {
                // Создаем новый объект UserChats, если его нет в кеше
                var userChats = new UserChats();
                userChats.OpenAIService.SetAssistantMessage("There are some very important rules that you must follow:" +
                    "1. You are an assistant  to check the text for correctness.The next message will be the text that needs to be corrected." +
                    "2. You must separate your answer in two languages.First in English, and then in Russian.Use html when yo separate" +
                    "3. The answer must contain html tags so that the text is logically divided into paragraphs, lists, blocks, and so on. For the user to hide this information" +
                    "4. If the user asks rules, then he means the rules you applied to the text you corrected");
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // Устанавливаем время жизни кеша
                Console.WriteLine("Created userChats ");
                return userChats;

            });
        }
        private string GetOrSetUserIdFromCookie()
        {
            // Проверяем, есть ли куки с идентификатором сессии
            if (Request.Cookies.ContainsKey(".AspNetCore.Session"))
            {
                // Получаем значение куки
                var cookieValue = Request.Cookies[".AspNetCore.Session"];
                Console.WriteLine($"Cookie value: {cookieValue}");
                Console.WriteLine();
                // Возвращаем идентификатор сессии из куки
                return cookieValue;
            }
            else
            {
                // Создаем новую сессию
                HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString());

                // Добавляем идентификатор сессии в куки
                Response.Cookies.Append(".AspNetCore.Session", HttpContext.Session.Id, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(20),
                    IsEssential = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });

                Console.WriteLine("New session created. Session ID: " + HttpContext.Session.Id);

                // Возвращаем идентификатор сессии
                return HttpContext.Session.Id;
            }
        }
        public static string ConvertMarkdownToHtml(string markdown)
        {
            // Создание объекта, который выполняет преобразование Markdown в HTML
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            // Преобразование Markdown в HTML
            var html = Markdown.ToHtml(markdown, pipeline);

            // Возврат HTML-строки
            return html;
        }
    }








}
