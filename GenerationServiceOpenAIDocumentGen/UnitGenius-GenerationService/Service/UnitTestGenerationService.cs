using System;
using System.Text;
using System.Collections.Generic;
using UnitGenius_GenerationService.Model;
using UnitGenius_GenerationService.Service.Abstraction;
using System.Net.Http;
using Newtonsoft.Json;

namespace UnitGenius_GenerationService.Service
{

    public class UnitTestGenerationService : IGenerationService
    {
        private readonly HttpClient _httpClient;

        public UnitTestGenerationService(string apikey)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/chat/completions");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apikey}");
        }


        public async Task<GenerationRequest> GenerateAsync(GenerationRequest request)
        {
            if (request.GenerationType != GenerationType.DocumentationOpenAI)
            {
                request.Status = Status.Failed;
                throw new ArgumentException("Invalid generation type. Only UnitTest generation is supported.");
            }
            if (string.IsNullOrEmpty(request.Code))
            {
                request.Status = Status.Failed;
                throw new ArgumentException("Code is required for generating unit tests.");
            }
            var messages = new[]
            {
                new {role = "user", content = $"Explain what this snippet does. Emphasize on the function in the snippet. Return only the function expanation:{request}"}
            };
            var data = new
            {
                model = "gpt-4o",
                messages = messages,
                temperature = 1,
                max_tokens = 500
            };
            string jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("completions", content);

            var result = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(result);
            string messageContent = jsonResponse.choices[0].message.content;
            request.Result = messageContent.ToString();
            request.Status = Status.Completed;
            return request;

        }


    }
}
