using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinalProjectResumeTuner
{
    internal class ChatGPTEngine
    {
        private HttpClient HTTPClient { get; set; }

        private string OPENAI_KEY = "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";

        private string OPENAI_MODEL = "gpt-4o-mini";

        private string API_ENDPOINT = "https://api.openai.com/v1/completions";
        public ChatGPTEngine()
        {
            HTTPClient = new HttpClient();
        }
        internal async Task<string> ProcessTheUserInput(string prompt)
        {
            var val = new AuthenticationHeaderValue("Bearer", OPENAI_KEY);
            HTTPClient.DefaultRequestHeaders.Authorization = val;
            var openAIPrompt = new
            {
                model = OPENAI_MODEL,
                prompt,
                temperature = 0.5,
                max_tokens = 1500,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var content = new StringContent(JsonSerializer.Serialize(openAIPrompt), Encoding.UTF8, "application/json");
            var response = await HTTPClient.PostAsync(API_ENDPOINT, content);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var choices = JsonDocument.Parse(jsonContent).RootElement.GetProperty("choices").GetRawText();
            var result = JsonDocument.Parse(Regex.Replace(choices, @"[\[\]]", string.Empty)).RootElement;
            return result.GetProperty("text").GetString();
        }
    }
}
