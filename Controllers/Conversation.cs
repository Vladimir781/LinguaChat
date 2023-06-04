using Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Chat.Controllers
{
    public class Conversation : BaseController
    {
        public Conversation(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public IActionResult Index()
        {
            // Each time the controller is accessed, delete the old UserChat object from the cache and create a new one
            _cache.Remove("UserChat" + GetOrSetUserIdFromCookie());
            Trace.TraceInformation("Starting Index");
            GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            return View();
        }

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> SetTopic([FromBody] MessageModel messageModel)
        {
            string message = messageModel.message;
            Trace.TraceInformation("Method called SendMessage.User message:" + message);
            var userChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await userChat.OpenAIService.GetResponseAsync("Let's start a dialogue on this topic '" + message + "'. Ask me questions about it, and I'll answer them." +
                "If I make a mistake in the dialogue, then correct me." +
                ""));

            // Return the response to the client
            return Content(response);
        }

        public async Task<ActionResult> SendMessage([FromBody] MessageModel messageModel)
        {
            string message = messageModel.message;
            Trace.TraceInformation("Method called SendMessage.User message:" + message);
            var userChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await userChat.OpenAIService.GetResponseAsync(message));

            // Return the response to the client
            return Content(response);
        }

    }
}
