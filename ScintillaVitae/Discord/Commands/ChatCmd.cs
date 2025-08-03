using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ScintillaVitae.Discord.Commands;

public class ChatCmd : ApplicationCommandModule<ApplicationCommandContext>
{

    [SlashCommand("chat", "Starts a chat thread")]
    public async Task TestCmdAsync(ThreadType threadType)
    {
        try
        {
            InteractionCallbackProperties callback = threadType switch
            {
                ThreadType.Private => InteractionCallback.DeferredMessage(NetCord.MessageFlags.Ephemeral),
                _ => InteractionCallback.DeferredMessage()
            };
            await Context.Interaction.SendResponseAsync(callback);

            GuildThreadProperties properties = new("Chat");
            if (threadType is ThreadType.Public)
                properties.ChannelType = NetCord.ChannelType.PublicGuildThread;
            var thread = await Context.Client.Rest.CreateGuildThreadAsync(Context.Channel.Id, properties);

            DiscordBot.MonitoredThreads.Add(thread.Id);

            await Context.Interaction.ModifyResponseAsync(message => message.WithContent($"New chat started. <#{thread.Id}>"));
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"{ex}");
            await Context.Interaction.ModifyResponseAsync(message => message.WithContent("Exception Occured."));
        }

    }

    public enum ThreadType
    {
        Public,
        Private
    }
}
