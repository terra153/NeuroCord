using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NeuroCord.Services
{
    public class NeuroService(IConfigurationRoot config) : INeuroService
    {
        private OrClient apiClient = new(apiUrl: "https://openrouter.ai/api/v1/chat/completions",
                            apiToken: config["connection:apiKey"]!);

        public async Task<string> AskNeuro(string content, string author)
        {
            try
            {
                var response = await apiClient.Chat.
                     WithModel(config["neuro:model"]!).
                     AddUserMessage($"{content} - пишет пользователь {author}")
                     .AddSystemMessage(config["neuro:prompt"]!)
                     .SendAsync();

                return response!.Choices[0].Message.ToString();
            }
            catch (System.Exception ex)
            {
                return $"{config["messages:apiError"]} {ex}";
            }
        }

        public void ResetContext()
        {
            //Если переинициализировать клиент - контекст сбросится
            apiClient = new(apiUrl: "https://openrouter.ai/api/v1/chat/completions",
                            apiToken: config["connection:apiKey"]!);
        }
    }
}