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
    Intents = GatewayIntents.GuildMessages | GatewayIntents.MessageContent,
    Logger = new ConsoleLogger()
});
ulong botId = 0;

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
discordClient.MessageCreate += async message =>
{
    //Игнорируемые пользователи
    if (config.Settings.IgnoreUserIds.Contains(message.Author.Id))
        return;

    //Игнорируемые сообщения
    if (config.Settings.IgnoreMessages.Contains(message.Content))
        return;

    //Не отвечать самому себе
    if (message.Author.Id == botId)
        return;

    //Игнорирование ботов
    if (config.Settings.IgnoreOtherBots && message.Author.IsBot)
        return;

    //Проверка ответа на все сообщения
    if (!config.Settings.AnswerAllMessages)
    {
        //Если не упоминается бот
        if (!message.MentionedUsers.Any(us => us.Id == botId))
            return;

        //Если нет reply боту
        if (message.ReferencedMessage != null &&
            message.ReferencedMessage.Author.Id != botId)
            return;
    }

    if (config.Settings.FullServerMode
    || message.ChannelId == config.Settings.DefaultChannelId)
    {
        var _neuro = serviceProvider.GetRequiredService<INeuroService>();

        NetCord.Rest.MessageProperties messageProps = config.Messages.Typing;
        var typingMessage = await discordClient.Rest.SendMessageAsync(config.Settings.DefaultChannelId, messageProps);

        var answer = await _neuro.AskNeuro(message.Content, message.Author.GlobalName ?? "Неизвестно");

        await message.ReplyAsync(answer);
        await typingMessage.DeleteAsync();
    }
};
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
    botId = args.User.Id;

    NetCord.Rest.MessageProperties messageProps = config.Messages.Hello;

    await discordClient.Rest.SendMessageAsync(config.Settings.DefaultChannelId, messageProps);
};
await discordClient.StartAsync();

await Task.Delay(-1);