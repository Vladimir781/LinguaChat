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
            // Create an object that performs Markdown to HTML conversion
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            Trace.TraceInformation("Response: " + markdown);
            // Converting Markdown to HTML
            var html = Markdown.ToHtml(markdown, pipeline);

            // Returning an HTML string
            return html;
        }
        protected UserChat GetOrSetUserChatFromCache(string userId)
        {
            return _cache.GetOrCreate("UserChat" + userId, entry =>
            {
                var currentUser = HttpContext.User; // get the current user
                var userChat = new UserChat();
                userChat.InitAsync(currentUser);
                userChat.OpenAIService.SetAssistantMessage("You are the assistant of the LinguaChat site, which is only for correcting the text and giving clarifications about the corrected text");
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // Set the cache lifetime
                Trace.WriteLine("Created user chat and set rule");
                return userChat;
            });
        }
        protected string GetOrSetUserIdFromCookie()
        {
            // check if there is a cookie with a session ID
            if (Request.Cookies.ContainsKey(".AspNetCore.Session"))
            {
                // Retrieve the value of the cookie
                var cookieValue = Request.Cookies[".AspNetCore.Session"];
                //Console.WriteLine($"Cookie value: {cookieValue}");
                System.Diagnostics.Trace.WriteLine($"Used cookie value.");
                //return the session identifier from the cookie
                return cookieValue;
            }
            else
            {
                // Create a new session
                HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString());

                // add a session identifier to the cookie
                Response.Cookies.Append(".AspNetCore.Session", HttpContext.Session.Id, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(15),
                    IsEssential = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
                Trace.WriteLine("New session created. Session ID: " + HttpContext.Session.Id);

                //return session identifier
                return HttpContext.Session.Id;
            }
        }
    }
}
