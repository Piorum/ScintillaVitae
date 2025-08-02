using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ScintillaVitae.Discord.Commands;

public class GoodbyeCmd
{
    public static async Task GoodbyeCmdAsync(ApplicationCommandContext Context)
    {
        try
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

            var response = $"Goodbye {Context.User.Username}.";

            await Context.Interaction.ModifyResponseAsync(message => message.WithContent(response));
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"{ex}");
        }

    }
}
