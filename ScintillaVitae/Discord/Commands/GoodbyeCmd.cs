using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScintillaVitae.Lms;

namespace ScintillaVitae.Discord.Commands;

public class GoodbyeCmd : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("goodbye", "Goodbye.")]
    public async Task GoodbyeCmdAsync()
    {
        try
        {
            //await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

            string? response = null;
            try
            {
                response = await Chat.CompleteChatAsync([$"{Context.User.Username} says Goodbye."]);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"{ex}");
            }

            if (response is not null)
            {
                await Context.Interaction.ModifyResponseAsync(message => message.WithContent(response));
            }
            else
            {
                await Context.Interaction.ModifyResponseAsync(message => message.WithContent("Model failed to respond."));
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"{ex}");
        }

    }
}
