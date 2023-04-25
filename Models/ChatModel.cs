using Microsoft.EntityFrameworkCore;
using OpenAIService;
using System.Security.Claims;
using Chat.Data;

namespace Chat.Models
{
    // class message
    public class MessageModel
    {
        public string message { get; set; }
    }
    public class AspNetUserCredit
    {
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public int? TotalUsedTokens { get; set; }
        public int? CreditGranted { get; set; }
    }
    public class UserChat
    {
        public IOpenAIService? OpenAIService { get; private set; }
        public ClaimsPrincipal? User { get; private set; }
        public AspNetUserCredit? aspNetUserCredit { get; set; } = null;
        public Task<UserChat> InitAsync(ClaimsPrincipal user)
        {
            OpenAIService = new OpenAIService.OpenAIService("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", "https://api.openai.com/v1/chat/completions", "gpt-3.5-turbo");
            User = user;
            OpenAIService.ResponseReceived += OnResponseReceived;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:linguachat.database.windows.net,1433;Initial Catalog=linguachat;Persist Security Info=False;User ID=Vladimir;Password=123456987tT.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            aspNetUserCredit = dbContext.GetAspNetUserCredit(user, dbContext).Result;
            return Task.FromResult(this);
        }
        public void OnResponseReceived(object sender, ResponseEventArgs e)
        {
            aspNetUserCredit.TotalUsedTokens += e.TotalTokens;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:linguachat.database.windows.net,1433;Initial Catalog=linguachat;Persist Security Info=False;User ID=Vladimir;Password=123456987tT.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            dbContext.SaveAspNetUserCredit(aspNetUserCredit, dbContext, e.PromptTokens + e.CompletionTokens);

            // some logic to use PromptTokens, CompletionTokens, and TotalTokens
            Console.WriteLine($"PromptTokens: {e.PromptTokens}, CompletionTokens: {e.CompletionTokens}, TotalTokens: {e.TotalTokens}");
            Console.WriteLine("{0,-25} {1}", "Property", "Value");
            Console.WriteLine("-------------------------");
            Console.WriteLine("{0,-25} {1}", "CreditGranted", aspNetUserCredit.CreditGranted);
            Console.WriteLine("{0,-25} {1}", "TotalUsedTokens", aspNetUserCredit.TotalUsedTokens);
        }




    }



}