using Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Chat.Controllers
{
    public class LearnNewWordsController : BaseController
    {
        public LearnNewWordsController(IMemoryCache memoryCache)
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
            string response = ConvertMarkdownToHtml(await userChat.OpenAIService.GetResponseAsync("Answer my question using the context of our correspondence. My question is:'" + message + "'." +
                "You must answer in two languages. First in English, and then in Russian. Response format:\r\n<div> English: your answer</div>\r\n<div> Russian: your answer</div>"));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> SelectTopic([FromBody] MessageModel messageModel)
        {
            _cache.Remove("UserChat" + GetOrSetUserIdFromCookie());
            GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            string message = messageModel.message;
            Trace.TraceInformation("Method called GetCorrects.User message:" + message);
            var UserChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("Give list of words linked with the subject "+message));

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


    }
}
