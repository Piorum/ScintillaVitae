using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScintillaVitae.GRPC;
using ScintillaVitae.Lms;

namespace ScintillaVitae.Discord.Commands;

public class Test : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("test", "Test")]
    public async Task TestCmdAsync()
    {
        try {
            //await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

            var messageServiceClient = await GrpcClientFactory.GetMessageServiceClient();

            var result = await messageServiceClient.StoreMessageAsync(new()
            {
                InteractionId = new()
                {
                    ServerId = 1,
                    ThreadId = 1
                },
                MessageContent = new()
                {
                    MessageRoll = Protos.Message.MessageRollProto.User,
                    Content = "Hello",
                    MessageId = 1,
                    Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                }
            });

            if(result.Success)
                await Context.Interaction.ModifyResponseAsync(message => message.WithContent("gRPC service returned true."));
            else
                await Context.Interaction.ModifyResponseAsync(message => message.WithContent("gRPC service returned false."));
        }
        catch (Exception ex) {
            await Console.Out.WriteLineAsync($"{ex}");
        }

    }
}
