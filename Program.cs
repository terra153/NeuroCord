using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NeuroCord.Modules;

var config = new ConfigurationBuilder()
    .AddJsonFile(Directory.GetCurrentDirectory() + "/settings.json", optional: false)
    .Build();

var discordClient = new GatewayClient(new BotToken(config["connection:botToken"]!), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages,
    Logger = new ConsoleLogger()
});

//Комманды
ApplicationCommandService<ApplicationCommandContext> applicationCommandService = new();
applicationCommandService.AddModules(typeof(DefaultCommandModule).Assembly);
await applicationCommandService.RegisterCommandsAsync(discordClient.Rest, discordClient.Id);

//Регистрируем сервисы
var services = new ServiceCollection();
services.AddSingleton(config);
var serviceProvider = services.BuildServiceProvider();

discordClient.InteractionCreate += async interaction =>
{
    if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
        return;

    var result = await applicationCommandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, discordClient));

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

await discordClient.StartAsync();
await Task.Delay(-1);