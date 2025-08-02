using System.Text;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScintillaVitae.GRPC;
using ScintillaVitae.Lms;

namespace ScintillaVitae.Discord.Commands;

public class Test : ApplicationCommandModule<ApplicationCommandContext>
{

    private static ulong count = 0;

    [SlashCommand("test", "Test")]
    public async Task TestCmdAsync()
    {
        try {
            //await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

            var messageServiceClient = await GrpcClientFactory.GetMessageServiceClient();

            var storeResult = await messageServiceClient.StoreMessageAsync(new()
            {
                InteractionId = new()
                {
                    ServerId = 0,
                    ThreadId = 0
                },
                MessageContent = new()
                {
                    MessageRole = Protos.Message.MessageRoleProto.User,
                    Content = "Hello World!",
                    MessageId = count,
                    Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                }
            });
            count++;

            if (storeResult.Success)
            {
                var result = await messageServiceClient.GetMessageHistoryAsync(new() { ServerId = 0, ThreadId = 0 });

                List<string> messages = [];
                foreach (var message in result.Messages)
                {
                    messages.Add($"{message.MessageId}:{message.MessageRole}:{message.Content}");
                }

                if (messages.Count > 0)
                    await Context.Interaction.ModifyResponseAsync(message => message.WithContent(string.Join(" || ",messages)));
                else
                    await Context.Interaction.ModifyResponseAsync(message => message.WithContent("Messages was empty."));
            }
            else
                await Context.Interaction.ModifyResponseAsync(message => message.WithContent("gRPC service returned false."));
        }
        catch (Exception ex) {
            await Console.Out.WriteLineAsync($"{ex}");
            await Context.Interaction.ModifyResponseAsync(message => message.WithContent("gRPC service failed."));
        }

    }
}
