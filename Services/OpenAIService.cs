using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace OpenAIService
{
    public interface IOpenAIService
    {
        Task<string> GetResponseAsync(string message);
        void SetAssistantMessage(string systemMessage);
        List<string> GetMessages();
        void ClearMessages();
        public event EventHandler<ResponseEventArgs> ResponseReceived;
    }

    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _modelId;
        private readonly List<Message> _messages;
        public event EventHandler<ResponseEventArgs> ResponseReceived;

        public string Id { get; private set; }
        public OpenAIService(string apiKey, string endpoint, string modelId)
        {
            _apiKey = apiKey;
            _endpoint = endpoint;
            _modelId = modelId;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _messages = new List<Message>();
        }
        public List<string> GetMessages()
        {
            return _messages.Select(x => x.Content).ToList();
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }

        public void SetAssistantMessage(string systemMessage)
        {
            var responseMessage = new Message() { Role = "user", Content = systemMessage };
            _messages.Add(responseMessage);
        }

        public async Task<string> GetResponseAsync(string message)
        {
            var requestData = new Request()
            {
                ModelId = _modelId,
                Messages = _messages
            };
            var responseMessage = new Message() { Role = "user", Content = message };
            _messages.Add(responseMessage);


            using var response = await _httpClient.PostAsJsonAsync(_endpoint, requestData);
            if (!response.IsSuccessStatusCode)
            {
                return "Error: " + response.StatusCode;
            }
            var responseData = await response.Content.ReadFromJsonAsync<ResponseData>();

            var usage = responseData?.Usage ?? new Usage();

            // some logic to get response from API
            var responseEventArgs = new ResponseEventArgs(usage.PromptTokens, usage.CompletionTokens, usage.TotalTokens);
            ResponseReceived?.Invoke(this, responseEventArgs);


            var choices = responseData?.Choices ?? new List<Choice>();
            if (choices.Count == 0)
            {
                return "No choices were returned by the API";
            }
            var choice = choices[0];
            var responseMessageData = choice.Message;
            _messages.Add(responseMessageData);
            var responseText = responseMessageData.Content.Trim();
            Console.WriteLine("responseText: " + responseText);
            return responseText;
        }

        private class Message
        {
            [JsonPropertyName("role")]
            public string Role { get; set; } = "";
            [JsonPropertyName("content")]
            public string Content { get; set; } = "";
        }

        private class Request
        {
            [JsonPropertyName("model")]
            public string ModelId { get; set; } = "";
            [JsonPropertyName("messages")]
            public List<Message> Messages { get; set; } = new();
        }

        private class ResponseData
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = "";
            [JsonPropertyName("object")]
            public string Object { get; set; } = "";
            [JsonPropertyName("created")]
            public ulong Created { get; set; }
            [JsonPropertyName("choices")]
            public List<Choice> Choices { get; set; } = new();
            [JsonPropertyName("usage")]
            public Usage Usage { get; set; } = new();
        }

        private class Choice
        {
            [JsonPropertyName("index")]
            public int Index { get; set; }
            [JsonPropertyName("message")]
            public Message Message { get; set; } = new();
            [JsonPropertyName("finish_reason")]
            public string FinishReason { get; set; } = "";
        }

        private class Usage
        {
            [JsonPropertyName("prompt_tokens")]
            public int PromptTokens { get; set; }
            [JsonPropertyName("completion_tokens")]
            public int CompletionTokens { get; set; }
            [JsonPropertyName("total_tokens")]
            public int TotalTokens { get; set; }
        }
    }
}

