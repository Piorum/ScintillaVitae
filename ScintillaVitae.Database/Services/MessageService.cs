using System.Text;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ScintillaVitae.Database.Data;
using ScintillaVitae.Database.Data.Models;
using ScintillaVitae.Protos.Message;

namespace ScintillaVitae.Database.Services;

public class MessageService(ChatContext context) : MessageServiceProto.MessageServiceProtoBase
{
    private readonly ChatContext _context = context;

    public override async Task<SuccessProto> StoreMessage(MessageProto messageProto, ServerCallContext context)
    {
        var logTask = Task.Run(async () =>
        {
            StringBuilder sb = new();
            sb.AppendLine("[Received Message]");
            sb.AppendLine($"[Interaction Id] {messageProto.InteractionId.ServerId}-{messageProto.InteractionId.ThreadId}");
            sb.AppendLine($"[Extras] {messageProto.MessageContent.MessageId}-{messageProto.MessageContent.Timestamp}");
            sb.AppendLine($"[Message] \"{messageProto.MessageContent.MessageRole}\":\"{messageProto.MessageContent.Content}\"");
            await Console.Out.WriteLineAsync($"{sb}");
        });

        var serverId = messageProto.InteractionId.ServerId;
        var threadId = messageProto.InteractionId.ThreadId;
        var interaction = await _context.Interactions
            .FirstOrDefaultAsync(i =>
                i.ServerId == serverId &&
                i.ThreadId == threadId);

        if (interaction == null)
        {
            await Console.Out.WriteLineAsync($"Creating New Interaction [{serverId}:{threadId}]");
            interaction = new()
            {
                ServerId = serverId,
                ThreadId = threadId
            };
        }

        var messageEntity = new MessageContent()
        {
            MessageId = messageProto.MessageContent.MessageId,
            Role = (MessageRole)messageProto.MessageContent.MessageRole,
            Content = messageProto.MessageContent.Content,
            Timestamp = DateTimeOffset.FromUnixTimeSeconds((long)messageProto.MessageContent.Timestamp).UtcDateTime,

            Interaction = interaction
        };

        await _context.MessageContents.AddAsync(messageEntity);
        await _context.SaveChangesAsync();

        await logTask;

        return new() { Success = true };
    }

    public override async Task<MessageHistoryProto> GetMessageHistory(InteractionIdProto interactionIdProto, ServerCallContext context)
    {
        var logTask = Task.Run(async () =>
        {
            StringBuilder sb = new();
            sb.AppendLine($"[Message History Requested]");
            sb.AppendLine($"[Interaction Id] {interactionIdProto.ServerId}-{interactionIdProto.ThreadId}");
            await Console.Out.WriteLineAsync($"{sb}");
        });

        var serverId = interactionIdProto.ServerId;
        var threadId = interactionIdProto.ThreadId;
        var history = await _context.Interactions
            .AsNoTracking()
            .Where(i => i.ServerId == serverId && i.ThreadId == threadId)
            .Include(i => i.MessageHistory.OrderBy(m => m.Timestamp))
            .FirstOrDefaultAsync();

        if (history is null)
            return new();

        var response = new MessageHistoryProto();
        response.Messages.AddRange(history.MessageHistory.Select(i =>
                new MessageContentProto()
                {
                    Content = i.Content,
                    MessageId = i.MessageId,
                    MessageRole = (MessageRoleProto)i.Role,
                    Timestamp = (ulong)new DateTimeOffset(i.Timestamp).ToUnixTimeSeconds()
                }));

        await logTask;

        return response;
    }
}
