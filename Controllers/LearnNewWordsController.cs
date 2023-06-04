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
            // Each time the controller is accessed, delete the old UserChat object from the cache and create a new one
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
                "You must answer in two languages. First in English, and then in Russian. Response format:\r\n<p> English: your answer</p>\r\n<p> Russian: your answer</p>"));

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
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("Give list of words linked with the subject "+message+ ". Give me the answer in a list"));

            // Return the response to the client
            return Content(response);
        }
        public async Task<ActionResult> GetWord([FromBody] MessageModel messageModel)
        {
            _cache.Remove("UserChat" + GetOrSetUserIdFromCookie());
            GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            string message = messageModel.message;
            Trace.TraceInformation("Method called GetCorrects.User message:" + message);
            var UserChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("What is meaning word '" + message + "'.Response format:\r\n<p> English: your answer</p>\r\n<p> Russian: your answer</p>"));
            Console.WriteLine("success"+ message);
            // Return the response to the client
            return Content(response);
        }


    }
}
