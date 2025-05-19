namespace MessageClient.HttpClients;

public sealed class IdentityApiClient(HttpClient http)
{
    public HttpClient Client { get; } = http;
}
