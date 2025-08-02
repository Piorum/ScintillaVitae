using ScintillaVitae.Protos.Message;
using Grpc.Net.Client;

namespace ScintillaVitae.GRPC;

public static class GrpcClientFactory{

    private static MessageServiceProto.MessageServiceProtoClient? messageServiceClient;

    public static Task<MessageServiceProto.MessageServiceProtoClient> GetMessageServiceClient()
    {
        if (messageServiceClient == null)
        {
            string databaseServiceUrl = Environment.GetEnvironmentVariable("DATABASE_SERVICE_URL") ?? throw new("Null Core Service URL");
            var channel = GrpcChannel.ForAddress(databaseServiceUrl);
            messageServiceClient = new(channel);
        }

        return Task.FromResult(messageServiceClient!);
    }

}