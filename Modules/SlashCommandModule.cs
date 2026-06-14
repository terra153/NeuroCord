using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetCord.Services.ApplicationCommands;
using NeuroCord.Services;

namespace NeuroCord.Modules
{
    public class SlashCommandModule(IConfigurationRoot config, INeuroService _neuro) : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SlashCommand("ping", "Проверка связи")]
        public string Pong() => $"{config["messages:ping"]} {Context.Client.Latency.Milliseconds}ms";
        [SlashCommand("reset-context", "Сбросить контекст")]
        public string Reset()
        {
            _neuro.ResetContext();

            return config["messages:resetContext"]!;
        }
    }
}
