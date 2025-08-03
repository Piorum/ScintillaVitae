using OpenAI.Chat;
using ScintillaVitae.Grpc;
using ScintillaVitae.Protos.Message;

namespace ScintillaVitae.Lms;

public static class Chat
{
    private static readonly ChatClient Client = ClientFactory.GetClient(Environment.GetEnvironmentVariable("CHAT_MODEL_NAME") ?? throw new("CHAT_MODEL_NAME is null"));

    public static async Task<string> CompleteChatAsync(params ChatMessage[] messages)
    {
        var completion = await Client.CompleteChatAsync(messages);
        string answer = completion.Value.Content[0].Text.Trim();

        return answer;
    }

    public static async Task<List<ChatMessage>> GetChatHistory(InteractionIdProto interactionId)
    {
        var msClient = await GrpcClientFactory.GetMessageServiceClient();

        var history = await msClient.GetMessageHistoryAsync(interactionId);

        List<ChatMessage> messages = [];
        foreach (var msg in history.Messages)
        {
            messages.Add
            (
                msg.MessageRole switch
                {
                    MessageRoleProto.Assistant => ChatMessage.CreateAssistantMessage(msg.Content),
                    MessageRoleProto.System => ChatMessage.CreateSystemMessage(msg.Content),
                    _ => ChatMessage.CreateUserMessage(msg.Content),
                }
            );
        }

        return messages;
    }

    public static async Task StoreMessage(MessageProto message)
    {
        var msClient = await GrpcClientFactory.GetMessageServiceClient();

        var promptStoreSuccess = await msClient.StoreMessageAsync(message);
        
        if (!promptStoreSuccess.Success) await Console.Out.WriteLineAsync($"Failed to store prompt");
    }
}
