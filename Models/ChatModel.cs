using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using OpenAIService;
using System.Security.Claims;

namespace Chat.Models
{
    // class message
    public class MessageModel
    {
        public string message { get; set; }
    }

    public class UserChats
    {
        public IOpenAIService OpenAIService { get; private set; }
        public ClaimsPrincipal User { get; private set; }
        public string Id { get; private set; }
        public UserChats()
        {
            OpenAIService = new OpenAIService.OpenAIService("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", "https://api.openai.com/v1/chat/completions", "gpt-3.5-turbo");
            User =new ClaimsPrincipal(new ClaimsIdentity()); // If user is null, create a new default ClaimsPrincipal object with an empty ClaimsIdentity
            //User = user ?? new ClaimsPrincipal(new ClaimsIdentity()); // If user is null, create a new default ClaimsPrincipal object with an empty ClaimsIdentity
            Id = Guid.NewGuid().ToString(); // Generate a new unique identifier
           // Console.WriteLine("ID UserChats"+ Id);
        }

    }





}