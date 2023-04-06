using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chat.Controllers
{

    public class ChatController : Controller
    {
        string topic = "";

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> SendMessage(string prompt)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", "Bearer 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");

                var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + prompt + "\",\"temperature\": 0.7,\"max_tokens\": 50}",
                    Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);
                HttpResponseMessage availableModels = await client.GetAsync("https://api.openai.com/v1/models");

                string availableModelsString = await availableModels.Content.ReadAsStringAsync();
                string responseString = await response.Content.ReadAsStringAsync();
                try
                {
                    var dyData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);
                    string guess = dyData!.choices[0].text;

                    return Json(new { success = true, message = guess });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Could not deserialize the JSON: {ex.Message}" });
                }
            }
            else
            {
                return Json(new { success = false, message = "You need to provide some input" });
            }
        }
        public async Task<JsonResult> SendTopic(string prompt)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                topic = prompt;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", "Bearer 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");
                prompt += "Give list of words linked with the subject " + prompt + ". Words should comma separated. Only words.";
                var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + prompt + "\",\"temperature\": 0.7,\"max_tokens\": 50}",
                    Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);

                
                string responseString = await response.Content.ReadAsStringAsync();
                try
                {
                    var dyData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);
                    string guess = dyData!.choices[0].text;

                    return Json(new { success = true, message = guess });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Could not deserialize the JSON: {ex.Message}" });
                }
            }
            else
            {
                return Json(new { success = false, message = "You need to provide some input" });
            }
        }
        public async Task<JsonResult> SendWord(string prompt)
        {
            if (!string.IsNullOrEmpty(prompt))
            {
                topic = prompt;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", "Bearer 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");
                prompt += "Give answer what is " + prompt + " from the field " + topic;
                var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + prompt + "\",\"temperature\": 0.7,\"max_tokens\": 50}",
                    Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);


                string responseString = await response.Content.ReadAsStringAsync();
                try
                {
                    var dyData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);
                    string guess = dyData!.choices[0].text;

                    return Json(new { success = true, message = guess });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Could not deserialize the JSON: {ex.Message}" });
                }
            }
            else
            {
                return Json(new { success = false, message = "You need to provide some input" });
            }
        }
    }

}



