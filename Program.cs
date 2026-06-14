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

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("settings.json", optional: false)
    .Build();

Configuration? config = configuration.GetSection("configuration").Get<Configuration>();

if (config == null)
    throw new Exception("Couldn't read configuration file");

var discordClient = new GatewayClient(new BotToken(config.Connection.BotToken), new GatewayClientConfiguration()
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
// discordClient.MessageCreate += async message =>
// {
//     // if (message.Author.Id == 1376943080321716456)
//     // {
//     //     return;
//     // }

//     if (message!.Content == "Печатаю...") return;

//     if (fullServerMode)
//     {
//         Console.WriteLine("FullServer Сырник спрашивает: " + text!.Content);
//         NeuroService.AskNeuro(message, text!.Content!);
//     }
//     else if (message.ChannelId == Consts.DEFAULT_CHANNEL)
//     {
//         Console.WriteLine("MyChannelOnly Сырник спрашивает: " + text!.Content);
//         NeuroService.AskNeuro(message, text!.Content!);
//     }
//     else
//     {
//         Console.WriteLine("Меня этот канал не касается");
//     }
//     return;
// };
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
    NetCord.Rest.MessageProperties messageProps = config.Messages.Hello!;

    await discordClient.Rest.SendMessageAsync(config.Settings.DefaultChannelId, messageProps);
};
await discordClient.StartAsync();

await Task.Delay(-1);