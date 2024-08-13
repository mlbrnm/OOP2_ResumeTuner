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
    internal class AIProcessor
    {
        // HttpClient to make the API call
        private HttpClient HTTPClient { get; set; }


        // I know this is bad! But I am just going to use it for the demo and then deactivate it.
        private string OPENAI_KEY = "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";

        // The endpoint to make the API call
        private string API_ENDPOINT = "https://api.openai.com/v1/completions";

        // Constructor
        public AIProcessor()
        {
            HTTPClient = new HttpClient();
        }

        // The main function that takes the user input and returns a response.
        internal async Task<string> SendToGPT(string prompt)
        {
            // Set the Authorization header
            var val = new AuthenticationHeaderValue("Bearer", OPENAI_KEY);
            HTTPClient.DefaultRequestHeaders.Authorization = val;

            // Sets the request body / inference parameters.
            var openAIPrompt = new
            {
                model = "gpt-4o",
                messages = new[]
                {
            new { role = "user", content = prompt }
        },
                temperature = 0.5,
                max_tokens = 3000,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            // Make the API call
            var content = new StringContent(JsonSerializer.Serialize(openAIPrompt), Encoding.UTF8, "application/json");
            var response = await HTTPClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var jsonContent = await response.Content.ReadAsStringAsync();

            // Debugging stuff.
            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            Console.WriteLine("Raw JSON response:");
            Console.WriteLine(jsonContent);

            // Process the response
            try
            {
                using (JsonDocument document = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = document.RootElement;

                    // Check if there's an error message
                    if (root.TryGetProperty("error", out JsonElement errorElement))
                    {
                        string errorMessage = errorElement.GetProperty("message").GetString();
                        Console.WriteLine($"API Error: {errorMessage}");
                        return $"Error: {errorMessage}";
                    }

                    // If no error, try to get the response
                    if (root.TryGetProperty("choices", out JsonElement choices))
                    {
                        var firstChoice = choices.EnumerateArray().First();
                        if (firstChoice.TryGetProperty("message", out JsonElement message))
                        {
                            if (message.TryGetProperty("content", out JsonElement contentt))
                            {
                                // Return the response if all goes well
                                return contentt.GetString();
                            }
                        }
                    }

                    // If we reach here, the JSON structure is unexpected
                    Console.WriteLine("Unexpected JSON structure. Printing all top-level properties:");
                    foreach (JsonProperty property in root.EnumerateObject())
                    {
                        Console.WriteLine($"Property: {property.Name}, Type: {property.Value.ValueKind}");
                    }
                }
            }
            // Catch any exceptions that occur while parsing the JSON
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            // If we reach here, something went wrong
            return "Failed to process the response";
        }
    }
}
