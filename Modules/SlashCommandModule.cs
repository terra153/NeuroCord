using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetCord.Services.ApplicationCommands;
using NeuroCord.Models;
using NeuroCord.Services;

namespace NeuroCord.Modules
{
    public class SlashCommandModule(Configuration config, INeuroService _neuro) : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SlashCommand("ping", "Проверка связи")]
        public string Pong() => $"{config.Messages.Ping} {Context.Client.Latency.Milliseconds}ms";
        [SlashCommand("reset-context", "Сбросить контекст")]
        public string Reset()
        {
            _neuro.ResetContext();

            return config.Messages.ResetContext;
        }
    }
}
