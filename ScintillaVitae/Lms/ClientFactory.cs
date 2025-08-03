using System.ClientModel;
using OpenAI;
using OpenAI.Chat;

namespace ScintillaVitae.Lms;

public static class ClientFactory
{
    private static readonly Uri baseUri;
    private static readonly ApiKeyCredential credential;

    private static readonly Dictionary<string, ChatClient> clientCache = [];

    static ClientFactory()
    {
        baseUri = new Uri(Environment.GetEnvironmentVariable("LMSTUDIO_ENDPOINT") ?? throw new("LMSTUDIO_ENDPOINT is null"));
        credential = new ApiKeyCredential("lm-studio");
    }

    public static ChatClient GetClient(string model)
    {
        clientCache.TryGetValue(model, out var client);

        if (client is null)
        {
            var options = new OpenAIClientOptions { Endpoint = baseUri };
            client = new ChatClient(
                model: model,
                credential: credential,
                options: options);
            clientCache.Add(model, client);
        }

        return client;
    }
    
    

}
