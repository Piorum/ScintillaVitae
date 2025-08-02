using Grpc.Core;
using ScintillaVitae.Database.Data;
using ScintillaVitae.Protos.Message;

namespace ScintillaVitae.Database.Services;

public class MessageService(ChatContext context) : MessageServiceProto.MessageServiceProtoBase
{
    private readonly ChatContext _context = context;

    public override async Task<SuccessProto> StoreMessage(MessageProto messageProto, ServerCallContext context)
    {
        var logTask = Task.Run(async () =>
        {
            await Console.Out.WriteLineAsync($"##Received Message##");
            await Console.Out.WriteLineAsync($"[Interaction Id] {messageProto.InteractionId.ServerId}-{messageProto.InteractionId.ThreadId}");
            await Console.Out.WriteLineAsync($"[Extras] {messageProto.MessageContent.MessageId}-{messageProto.MessageContent.Timestamp}");
            await Console.Out.WriteLineAsync($"[Message] \"{messageProto.MessageContent.MessageRoll}\":\"{messageProto.MessageContent.Content}\"");
        });

        await logTask;

        return new() { Success = false };
    }

    public override async Task<MessageHistoryProto> GetMessageHistory(InteractionIdProto interactionIdProto, ServerCallContext context)
    {
        var logTask = Task.Run(async () =>
        {
            await Console.Out.WriteLineAsync($"##Message History Requested##");
            await Console.Out.WriteLineAsync($"[Interaction Id] {interactionIdProto.ServerId}-{interactionIdProto.ThreadId}");
        });

        MessageHistoryProto messageHistory = new();
        messageHistory.Messages.AddRange([]);

        await logTask;

        return messageHistory;
    }
}
