namespace MessageClient.HttpClients;

public sealed class MessagesApiClient(HttpClient http)
{
    public HttpClient Client { get; } = http;
}
