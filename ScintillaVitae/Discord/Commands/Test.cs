using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ScintillaVitae.Discord.Commands;

public class Test : ApplicationCommandModule<ApplicationCommandContext>
{

    [SlashCommand("test", "Test")]
    public async Task TestCmdAsync()
    {
        try
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

            var thread = await Context.Client.Rest.CreateGuildThreadAsync(Context.Channel.Id, new("Chat"));
            await thread.SendMessageAsync(new() { Content = "## [Start of the Chat]" });

            DiscordBot.MonitoredThreads.Add(thread.Id);

            await Context.Interaction.ModifyResponseAsync(message => message.WithContent($"New chat started. <#{thread.Id}>"));

            //await Task.Delay(3 * 1000);

            //StringBuilder sb = new();
            //for (int i = 0; i < 100; i++)
            //{
            //    sb.Append("Long Text");
            //}

            //var followup1 = await Context.Interaction.SendFollowupMessageAsync(new() { Content = "Test Followup Message", AllowedMentions = AllowedMentionsProperties.None });
            //await Task.Delay(3 * 1000);
            //await Context.Interaction.ModifyFollowupMessageAsync(followup1.Id, message => message.WithContent("Modified Followup Message"));

            //var messageServiceClient = await GrpcClientFactory.GetMessageServiceClient();

            //var history = await messageServiceClient.GetMessageHistoryAsync(new() { ServerId = 0, ThreadId = 0 });

            /*await Context.Client.Rest.ModifyGuildChannelAsync(0, options =>
            {
                options.Name = "New Name";
            });*/

            /*List<string> messages = [];
            foreach (var message in history.Messages)
            {
                messages.Add($"{message.MessageId}:{message.MessageRole}:{message.Content}:{message.Timestamp}");
            }

            if (messages.Count > 0)
                await Context.Interaction.ModifyResponseAsync(message => message.WithContent(string.Join(" || ",messages)));
            else
                await Context.Interaction.ModifyResponseAsync(message => message.WithContent("Messages was empty."));*/
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"{ex}");
            await Context.Interaction.ModifyResponseAsync(message => message.WithContent("Exception Occured."));
        }

    }
}
