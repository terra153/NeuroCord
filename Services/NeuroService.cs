using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NeuroCord.Models;

namespace NeuroCord.Services
{
    public class NeuroService(Configuration config) : INeuroService
    {
        private OrClient apiClient = new(apiUrl: "https://openrouter.ai/api/v1/chat/completions",
                            apiToken: config.Connection.ApiKey);

        public async Task<string> AskNeuro(string content, string author)
        {
            try
            {
                var response = await apiClient.Chat.
                     WithModel(config.Neuro.Model).
                     AddUserMessage($"{content} - пишет пользователь {author}")
                     .AddSystemMessage(config.Neuro.Prompt)
                     .SendAsync();

                return response!.Choices[0].Message.ToString();
            }
            catch (System.Exception ex)
            {
                return $"{config.Messages.ApiError} {ex}";
            }
        }

        public void ResetContext()
        {
            //Если переинициализировать клиент - контекст сбросится
            apiClient = new(apiUrl: "https://openrouter.ai/api/v1/chat/completions",
                            apiToken: config.Connection.ApiKey);
        }
    }
}