using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetCord.Services.ApplicationCommands;

namespace NeuroCord.Modules
{
    public class DefaultCommandModule(IConfigurationRoot config) : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SlashCommand("ping", "Проверка связи")]
        public string Pong() => $"{config["messages:ping"]} {Context.Client.Latency.Milliseconds}ms";
    }
}