using Chat.Models;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Chat.Controllers
{
    public class BaseController : Controller
    {
        protected IMemoryCache _cache;
        protected static string ConvertMarkdownToHtml(string markdown)
        {
            // Создание объекта, который выполняет преобразование Markdown в HTML
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            Trace.TraceInformation("Response: " + markdown);
            // Преобразование Markdown в HTML
            var html = Markdown.ToHtml(markdown, pipeline);

            // Возврат HTML-строки
            return html;
        }
        protected UserChat GetOrSetUserChatFromCache(string userId)
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
        protected string GetOrSetUserIdFromCookie()
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
    }
}
