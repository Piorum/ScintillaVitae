using System.Collections.Concurrent;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using OpenAI.Chat;
using ScintillaVitae.Grpc;
using ScintillaVitae.Lms;

namespace ScintillaVitae.Discord;

public static class DiscordBot
{
    private static readonly ApplicationCommandService<ApplicationCommandContext> applicationCommandService;
    private static readonly GatewayClient discordClient;

    public static readonly ConcurrentBag<ulong> MonitoredThreads = [];

    static DiscordBot()
    {
        var rawToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? throw new("Null Bot Token");
        var botToken = new BotToken(rawToken);
        var gatewayConfiguration = new GatewayClientConfiguration()
        {
            Intents = GatewayIntents.All,
            Logger = new ConsoleLogger()
        };
        discordClient = new(botToken, gatewayConfiguration);

        discordClient.InteractionCreate += async interaction => await InteractionHandlerAsync(interaction);
        discordClient.MessageCreate += async message =>
        {
            if (MonitoredThreads.Contains(message.ChannelId) && !message.Author.IsBot)
            {
                await Respond(message);
                return;
            }
        };

        applicationCommandService = new();

        applicationCommandService.AddModules(typeof(Program).Assembly);
    }

    public static async Task StartAsync()
    {
        //Uncomment to clear commands
        //await ClearApplicationCommandsAsync();
        await applicationCommandService.RegisterCommandsAsync(discordClient.Rest, discordClient.Id);
        await discordClient.StartAsync();
    }

    public static async Task StopAsync()
    {
        await discordClient.CloseAsync();
    }

    //Leave To Allow Clearing Of Application Commands
    private static async Task ClearApplicationCommandsAsync()
    {
        List<Task> DeletionTasks = [];

        //Getting Global Slash Commands
        var globalCommands = await discordClient.Rest.GetGlobalApplicationCommandsAsync(discordClient.Id);
        foreach (var command in globalCommands)
        {
            var deletionTask = command.DeleteAsync();
            DeletionTasks.Add(deletionTask);
        }

        //Deleting All Slash Commands
        await Task.WhenAll(DeletionTasks);
    }

    private static async Task InteractionHandlerAsync(Interaction interaction)
    {
        if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
            return;

        //Limit to guilds only
        if (interaction.Guild is null)
        {
            await interaction.SendResponseAsync(InteractionCallback.Message("This application's commands are only useable in a guild context."));
            return;
        }

        //Executes Delegate Passing Application Command Context
        var result = await applicationCommandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, discordClient));

        if (result is not IFailResult failResult)
            return;
        try
        {
            await Console.Out.WriteLineAsync($"Interaction execution has failed.\n{failResult.Message}");
            await interaction.SendResponseAsync(InteractionCallback.Message("Interaction execution has failed."));
        }
        catch { }
    }

    private static async Task Respond(Message message)
    {
        var msClient = await GrpcClientFactory.GetMessageServiceClient();

        var serverId = (ulong)message.GuildId!;
        var threadId = message.ChannelId;
        var history = await msClient.GetMessageHistoryAsync(new() { ServerId = serverId, ThreadId = threadId });

        List<ChatMessage> messages = [];
        foreach (var msg in history.Messages)
        {
            messages.Add
            (
                msg.MessageRole switch
                {
                    Protos.Message.MessageRoleProto.Assistant => ChatMessage.CreateAssistantMessage(msg.Content),
                    Protos.Message.MessageRoleProto.System => ChatMessage.CreateSystemMessage(msg.Content),
                    _ => ChatMessage.CreateUserMessage(msg.Content),
                }
            );
        }

        messages.Add(ChatMessage.CreateUserMessage(message.Content));

        var response = await Chat.CompleteChatAsync([.. messages]);

        var promptStoreSuccess = await msClient.StoreMessageAsync(new()
        {
            InteractionId = new()
            {
                ServerId = serverId,
                ThreadId = threadId
            },
            MessageContent = new()
            {
                MessageRole = Protos.Message.MessageRoleProto.User,
                Content = message.Content,
                MessageId = message.Id,
                Timestamp = (ulong)message.CreatedAt.ToUnixTimeSeconds()
            }
        });
        if (!promptStoreSuccess.Success) await Console.Out.WriteLineAsync($"Failed to store prompt");

        var followupMsg = await discordClient.Rest.SendMessageAsync(threadId, response);

        var responseStoreSuccess = await msClient.StoreMessageAsync(new()
        {
            InteractionId = new()
            {
                ServerId = serverId,
                ThreadId = threadId
            },
            MessageContent = new()
            {
                MessageRole = Protos.Message.MessageRoleProto.Assistant,
                Content = response,
                MessageId = followupMsg.Id,
                Timestamp = (ulong)followupMsg.CreatedAt.ToUnixTimeSeconds()
            }
        });
        if (!responseStoreSuccess.Success) await Console.Out.WriteLineAsync($"Failed to store response");
    }
}
