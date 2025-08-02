using OpenAI.Chat;

namespace ScintillaVitae.Lms;

public static class Chat
{
    private static readonly ChatClient Client = ClientFactory.GetClient(Environment.GetEnvironmentVariable("CHAT_MODEL_NAME") ?? throw new ("CHAT_MODEL_NAME is null"));

    public static async Task<string> CompleteChatAsync(params ChatMessage[] messages)
    {
        var completion = await Client.CompleteChatAsync(messages);
        string answer = completion.Value.Content[0].Text.Trim();

        return answer;
    }
}
