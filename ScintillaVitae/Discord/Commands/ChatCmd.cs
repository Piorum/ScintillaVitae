using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ScintillaVitae.Discord.Commands;

public class ChatCmd : ApplicationCommandModule<ApplicationCommandContext>
{

    [SlashCommand("chat", "Starts a chat thread")]
    public async Task TestCmdAsync()
    {
        try
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

            var thread = await Context.Client.Rest.CreateGuildThreadAsync(Context.Channel.Id, new("Chat"));

            DiscordBot.MonitoredThreads.Add(thread.Id);

            await Context.Interaction.ModifyResponseAsync(message => message.WithContent($"New chat started. <#{thread.Id}>"));
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"{ex}");
            await Context.Interaction.ModifyResponseAsync(message => message.WithContent("Exception Occured."));
        }

    }
}
