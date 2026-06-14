using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NeuroCord.Models;
using NeuroCord.Modules;
using NeuroCord.Services;
using Newtonsoft.Json;

var config = new ConfigurationBuilder()
    .AddJsonFile(Directory.GetCurrentDirectory() + "/settings.json", optional: false)
    .Build();
string settingsString = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/settings.json");
var settings = JsonConvert.DeserializeObject<Settings>(settingsString)!;

var discordClient = new GatewayClient(new BotToken(config["connection:botToken"]!), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages,
    Logger = new ConsoleLogger()
});

//Комманды
ApplicationCommandService<ApplicationCommandContext> applicationCommandService = new();
applicationCommandService.AddModules(typeof(SlashCommandModule).Assembly);
await applicationCommandService.RegisterCommandsAsync(discordClient.Rest, discordClient.Id);

//Регистрируем сервисы
var services = new ServiceCollection();
services.AddSingleton(config);
//Синглтон для сохранения контекста диалога
services.AddSingleton<INeuroService, NeuroService>();
var serviceProvider = services.BuildServiceProvider();

//События
discordClient.InteractionCreate += async interaction =>
{
    if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
        return;

    var result = await applicationCommandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, discordClient), serviceProvider);

    if (result is not IFailResult failResult)
        return;

    try
    {
        await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
    }
    catch
    {
    }
};
discordClient.Ready += async args =>
{
    NetCord.Rest.MessageProperties messageProps = config["messages:hello"]!;

    await discordClient.Rest.SendMessageAsync(settings.DefaultChannelId, messageProps);
};
await discordClient.StartAsync();

await Task.Delay(-1);