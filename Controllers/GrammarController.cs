using Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Markdig;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace Chat.Controllers
{
    public class GrammarController : BaseController
    {

        public GrammarController(IMemoryCache memoryCache)
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
            string response = ConvertMarkdownToHtml(await userChat.OpenAIService.GetResponseAsync("Answer my question using the context of our correspondence. My question is:'" + message+ "'." +
                "You must answer in two languages. First in English, and then in Russian. Response format:<p> English: your answer</p><p> Russian: your answer</p>"));

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
                "You must answer in two languages. First in English, and then in Russian. Response format:<p> Corrected text in English: your answer</p><p> Translated text in Russian: your answer</p>"));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        public async Task<ActionResult> GetRules([FromBody] MessageModel messageModel)
        {
            Trace.WriteLine("Method called GetRules.");
            var UserChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("Why did you fix it that way and give recommendations and how it was possible to write the text differently and why ?" +
                "You must answer in two languages. First in English, and then in Russian. Response format:<p> English: your answer</p><p> Russian: your answer</p>"));

            // Return the response to the client
            return Content(response);
        }
        [HttpPost]
        public async Task<ActionResult> GetExamples([FromBody] MessageModel messageModel)
        {
            Trace.WriteLine("Method called GetExamples.");
            var UserChat = GetOrSetUserChatFromCache(GetOrSetUserIdFromCookie());

            // Call the OpenAI service to get a response
            string response = ConvertMarkdownToHtml(await UserChat.OpenAIService.GetResponseAsync("I don't understand.Give similar examples." +
                "You must answer in two languages. First in English, and then in Russian. Response format:<p> English: your answer</p><p> Russian: your answer</p>"));

            // Return the response to the client
            return Content(response);
        }



    }








}
