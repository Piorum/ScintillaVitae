using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

namespace ScintillaVitae.Lms;

public static class ClientFactory
{
    private static readonly Uri baseUri;
    private static readonly ApiKeyCredential credential;

    static ClientFactory()
    {
        baseUri = new Uri(Environment.GetEnvironmentVariable("LMSTUDIO_ENDPOINT") ?? throw new("LMSTUDIO_ENDPOINT is null"));
        credential = new ApiKeyCredential("lm-studio");
    }

    public static ChatClient GetClient(string model)
    {
        var options = new OpenAIClientOptions { Endpoint = baseUri };
        var client = new ChatClient(
            model: model,
            credential: credential,
            options: options);
        return client;
    }
    
    

}
