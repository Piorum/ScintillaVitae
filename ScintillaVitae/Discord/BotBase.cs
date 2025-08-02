using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using ScintillaVitae.Discord.Commands;

namespace ScintillaVitae.Discord;

public static class BotBase
{
    private static readonly ApplicationCommandService<ApplicationCommandContext> applicationCommandService;
    private static readonly GatewayClient discordClient;

    static BotBase(){
        var rawToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? throw new("Null Bot Token");
        var botToken = new BotToken(rawToken);
        var gatewayConfiguration = new GatewayClientConfiguration()
        {
            Intents = GatewayIntents.Guilds,
            Logger = new ConsoleLogger()
        };
        discordClient = new(botToken, gatewayConfiguration);

        discordClient.InteractionCreate += async interaction => await InteractionHandlerAsync(interaction);

        applicationCommandService = new();

        applicationCommandService.AddSlashCommand("hello", "Hello!", HelloCmd.HelloCmdAsync);
        applicationCommandService.AddSlashCommand("goodbye", "Goodbye.", GoodbyeCmd.GoodbyeCmdAsync);
    }

    public static async Task StartAsync(){
        //Uncomment to clear commands
        //await ClearApplicationCommandsAsync();
        await applicationCommandService.RegisterCommandsAsync(discordClient.Rest, discordClient.Id);
        await discordClient.StartAsync();
    }

    public static async Task StopAsync(){
        await discordClient.CloseAsync();
    }

    //Leave To Allow Clearing Of Application Commands
    private static async Task ClearApplicationCommandsAsync(){
        List<Task> DeletionTasks = [];

        //Getting Global Slash Commands
        var globalCommands = await discordClient.Rest.GetGlobalApplicationCommandsAsync(discordClient.Id);
        foreach(var command in globalCommands){
            var deletionTask = command.DeleteAsync();
            DeletionTasks.Add(deletionTask);
        }

        //Deleting All Slash Commands
        await Task.WhenAll(DeletionTasks);
    }

    private static async Task InteractionHandlerAsync(Interaction interaction){
        if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
            return;

        //Limit to guilds only
        if (interaction.Guild is null){
            await interaction.SendResponseAsync(InteractionCallback.Message("This application's commands are only useable in a guild context."));
            return;
        }

        //Executes Delegate Passing Application Command Context
        var result = await applicationCommandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, discordClient));

        if (result is not IFailResult failResult)
            return;
        try{
            await Console.Out.WriteLineAsync($"Interaction execution has failed.\n{failResult.Message}");
            await interaction.SendResponseAsync(InteractionCallback.Message("Interaction execution has failed."));
        } catch {}
    }

}
