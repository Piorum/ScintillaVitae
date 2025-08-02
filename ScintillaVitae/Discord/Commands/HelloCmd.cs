using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ScintillaVitae.Discord.Commands;

public class HelloCmd
{
    public static async Task HelloCmdAsync(ApplicationCommandContext Context)
    {
        try
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

            var response = $"Hello {Context.User.Username}!";

            await Context.Interaction.ModifyResponseAsync(message => message.WithContent(response));
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"{ex}");
        }

    }
}
