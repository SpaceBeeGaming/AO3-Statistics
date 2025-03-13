using System.Net;

using Microsoft.Extensions.Logging;

namespace AO3Statistics;
public sealed class HttpHelper(HttpClient httpClient, ILogger<HttpHelper> logger)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<HttpHelper> logger = logger;

    public async Task<HttpResponseMessage> RedirectGet(Uri uri)
    {
        HttpResponseMessage getResponse = await httpClient.GetAsync(uri);
        while (getResponse.StatusCode is HttpStatusCode.Moved)
        {
            logger.LogInformation("Redirect 301");
            getResponse = await httpClient.GetAsync(getResponse.Headers.Location);
        }

        return getResponse;
    }
    public async Task<HttpResponseMessage> RedirectPost(Uri uri, Dictionary<string, string> content)
    {
        FormUrlEncodedContent urlContent = new(content);
        HttpResponseMessage postResponse = await httpClient.PostAsync(uri, urlContent);
        while (postResponse.StatusCode is HttpStatusCode.Moved) // Should Be RedirectKeepVerb (307), but Cloudflare returns 301.
        {
            logger.LogInformation("Redirect 301");
            postResponse = await httpClient.PostAsync(postResponse.Headers.Location, urlContent);
        }

        while (postResponse.StatusCode is HttpStatusCode.Redirect)
        {
            logger.LogInformation("Redirect 302");
            postResponse = await httpClient.GetAsync(postResponse.Headers.Location);
        }

        return postResponse;
    }
}
