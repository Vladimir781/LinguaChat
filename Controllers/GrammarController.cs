using Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Markdig;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

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
            // При каждом обращении к контроллеру удаляем старый объект UserChat из кеша и создаем новый
            _cache.Remove("UserChat" + GetOrSetUserIdFromCookie());
            Trace.TraceInformation("Starting Index");
            GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            return View();
        }

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> SendMessage([FromBody] MessageModel messageModel)
        {
            string message = messageModel.message;
            Trace.TraceInformation("Method called SendMessage.User message:" + message);
            var userChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await userChat.OpenAIService.GetResponseAsync("Answer my question using the context of our correspondence. My question is:'" + message+ "'." +
                "You must answer in two languages. First in English, and then in Russian. Response format:\r\n<div> English: your answer</div>\r\n<div> Russian: your answer</div>"));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> GetCorrects([FromBody] MessageModel messageModel)
        {
            _cache.Remove("UserChat" + GetOrSetUserIdFromCookie());
            GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            string message = messageModel.message;
            Trace.TraceInformation("Method called GetCorrects.User message:" + message);
            var UserChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("Correct this text:'" + message+ "'." +
                "You must answer in two languages. First in English, and then in Russian. Response format:\r\n<div> English: your answer</div>\r\n<div> Russian: your answer</div>"));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        public async Task<ActionResult> GetRules([FromBody] MessageModel messageModel)
        {
            Trace.WriteLine("Method called GetRules.");
            var UserChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("What rules did you apply to the corrected text. Also give recommendations and how it was possible to write the text differently and why." +
                "You must answer in two languages. First in English, and then in Russian. Response format:\r\n<div> English: your answer</div>\r\n<div> Russian: your answer</div>"));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        public async Task<ActionResult> GetExamples([FromBody] MessageModel messageModel)
        {
            Trace.WriteLine("Method called GetExamples.");
            var UserChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("Give a few sentences using the rules you applied. " +
                "You must answer in two languages. First in English, and then in Russian. Response format:\r\n<div> English: your answer</div>\r\n<div> Russian: your answer</div>"));

            // Return the response to the client
            return Content(response);
        }

        private UserChat GetOrSetUserChatFromCache(string userId)
        {
            return _cache.GetOrCreate("UserChat" + userId, entry =>
            {
                var currentUser = HttpContext.User; // получить текущего пользователя
                var userChat = new UserChat();
                userChat.InitAsync(currentUser);
                userChat.OpenAIService.SetAssistantMessage("You are the assistant of the LinguaChat site, which is only for correcting the text and giving clarifications about the corrected text");
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // Устанавливаем время жизни кеша
                Trace.WriteLine("Created user chat and set rule");
                return userChat;
            });
        }
        private string GetOrSetUserIdFromCookie()
        {
            // Проверяем, есть ли куки с идентификатором сессии
            if (Request.Cookies.ContainsKey(".AspNetCore.Session"))
            {
                // Получаем значение куки
                var cookieValue = Request.Cookies[".AspNetCore.Session"];
                //Console.WriteLine($"Cookie value: {cookieValue}");
                System.Diagnostics.Trace.WriteLine($"Used cookie value.");
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
                    Expires = DateTimeOffset.Now.AddMinutes(15),
                    IsEssential = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
                Trace.WriteLine("New session created. Session ID: " + HttpContext.Session.Id);

                // Возвращаем идентификатор сессии
                return HttpContext.Session.Id;
            }
        }
        public static string ConvertMarkdownToHtml(string markdown)
        {
            // Создание объекта, который выполняет преобразование Markdown в HTML
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            Trace.TraceInformation("Response: " + markdown);
            // Преобразование Markdown в HTML
            var html = Markdown.ToHtml(markdown, pipeline);

            // Возврат HTML-строки
            return html;
        }
    }








}
