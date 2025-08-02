using System.ClientModel;
using System.Runtime.CompilerServices;
using OpenAI;
using OpenAI.Chat;

namespace ScintillaVitae.Lms;

public class ChatBase
{
    private static ChatClient? _client;

    protected static ChatClient Client
    {
        get { return _client!; }
        private set { _client = value; }
    }

    [ModuleInitializer]
    public static void Init()
    {
        var baseUri = new Uri(Environment.GetEnvironmentVariable("LMSTUDIO_ENDPOINT") ?? throw new ("LMSTUDIO_ENDPOINT is null"));
        var credential = new ApiKeyCredential("lm-studio");
        var options = new OpenAIClientOptions { Endpoint = baseUri };
        _client = new ChatClient(
            model: "neonmaid-12b",
            credential: credential,
            options: options);
    }

}
