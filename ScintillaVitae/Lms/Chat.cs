using OpenAI.Chat;

namespace ScintillaVitae.Lms;

public class Chat : ChatBase
{
    public static async Task<string> CompleteChatAsync(params ChatMessage[] messages)
    {
        var completion = await Client.CompleteChatAsync(messages);
        string answer = completion.Value.Content[0].Text.Trim();

        return answer;
    }
}
