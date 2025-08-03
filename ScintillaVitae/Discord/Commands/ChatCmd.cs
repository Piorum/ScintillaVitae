using NetCord;
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
            GuildThread? thread = null;
            GuildThreadProperties properties = new("Chat");
            switch (threadType)
            {
                case ThreadType.Public:
                    await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

                    properties.ChannelType = ChannelType.PublicGuildThread;
                    break;

                case ThreadType.Private:
                    await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
                    break;
            }

            thread = await Context.Client.Rest.CreateGuildThreadAsync(Context.Channel.Id, properties);

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
