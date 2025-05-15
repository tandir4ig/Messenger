namespace MessageClient.Extensions;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessage Clone(this HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri)
        {
            Content = req.Content,
            Version = req.Version,
        };
        foreach (var h in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(h.Key, h.Value);
        }

        foreach (var p in req.Options)
        {
            clone.Options.TryAdd(p.Key, p.Value);
        }

        return clone;
    }
}
