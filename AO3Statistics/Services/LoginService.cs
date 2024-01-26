using System.Net;

using AO3Statistics.Enums;
using AO3Statistics.ExtensionMethods;
using AO3Statistics.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics.Services;
public class LoginService(ILogger<LoginService> logger,
    IOptions<UrlOptions> urlOptions,
    IOptions<UserOptions> userOptions,
    HtmlNavigationService htmlNavigationService,
    HttpClient httpClient)
{
    private readonly ILogger<LoginService> logger = logger;
    private readonly HtmlNavigationService htmlNavigationService = htmlNavigationService;
    private readonly HttpClient httpClient = httpClient;
    private readonly IOptions<UrlOptions> urlOptions = urlOptions;
    private readonly IOptions<UserOptions> userOptions = userOptions;

    /// <summary>
    /// Log in to AO3.
    /// </summary>
    /// <returns>Returns <see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException"> Thrown when login is attempted while already logged in.</exception>
    public async Task<bool> LoginAsync()
    {
        // Check that we have a username and a password.
        if (userOptions.Value.Username.IsNullOrWhitespace() || userOptions.Value.Password.IsNullOrWhitespace())
        {
            logger.LogError("Username or password is missing.");
            return false;
        }

        // HTTP GET the login page.
        HttpResponseMessage getResponse = await httpClient.GetAsync(urlOptions.Value.LoginUrl);
        while (getResponse.StatusCode is HttpStatusCode.Moved)
        {
            getResponse = await httpClient.GetAsync(getResponse.Headers.Location);
        }

        if (!getResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Getting to the Login page. {StatusCode} {Reason}", getResponse.StatusCode, getResponse.ReasonPhrase);
            return false;
        }

        // Construct the content for the login attempt.
        htmlNavigationService.LoadDocument(await getResponse.Content.ReadAsStreamAsync());

        // Check if we're already logged in. No need to try to do it twice.
        // This condition should never be met unless this program has a bug.
        if (htmlNavigationService.IsDocumentLoaded && htmlNavigationService.GetLoggedInStatus() is LoggedInStatus.LoggedId)
        {
            logger.LogInformation("Already logged in.");
            return true;
        }

        Dictionary<string, string> content = new()
        {
            { "authenticity_token", htmlNavigationService.GetLoginFormAuthenticityToken() },
            { "user[login]", userOptions.Value.Username },
            { "user[password]", userOptions.Value.Password }
        };

        // HTTP POST the credentials.
        HttpResponseMessage postResponse = await httpClient.PostAsync(urlOptions.Value.LoginUrl, new FormUrlEncodedContent(content));
        while (postResponse.StatusCode is HttpStatusCode.Moved) // Should Be RedirectKeepVerb (307), but Cloudflare returns 301.
        {
            postResponse = await httpClient.PostAsync(postResponse.Headers.Location, new FormUrlEncodedContent(content));
        }

        while (postResponse.StatusCode is HttpStatusCode.Redirect)
        {
            postResponse = await httpClient.GetAsync(postResponse.Headers.Location);
        }

        if (!postResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Logging in. {StatusCode} {Reason}", postResponse.StatusCode, postResponse.ReasonPhrase);
            return false;
        }

        // Check if we logged in successfully or not.
        htmlNavigationService.LoadDocument(await postResponse.Content.ReadAsStreamAsync());
        if (htmlNavigationService.GetLoggedInStatus() is LoggedInStatus.LoggedId)
        {
            logger.LogInformation("Successfully logged in as {Username}", userOptions.Value.Username);
            return true;
        }
        else
        {
            logger.LogError("Error Logging into AO3. Check credentials.");
            return false;
        }
    }

    /// <summary>
    /// Logs out of AO3.
    /// </summary>
    /// <returns>Returns <see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when logout is attempted while logged out.</exception>
    public async Task LogoutAsync()
    {
        // HTTP GET the logout page.
        HttpResponseMessage getResponse = await httpClient.GetAsync(urlOptions.Value.LogOutUrl);
        while (getResponse.StatusCode is HttpStatusCode.Moved)
        {
            getResponse = await httpClient.GetAsync(getResponse.Headers.Location);
        }

        if (!getResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Getting to the Logout page. {StatusCode} {Reason}", getResponse.StatusCode, getResponse.ReasonPhrase);
        }

        // Construct the content for the logout attempt.
        htmlNavigationService.LoadDocument(await getResponse.Content.ReadAsStreamAsync());

        // Check if we're already logged out. No need to try to do it twice.
        // This condition should never be met unless this program has a bug.
        if (htmlNavigationService.IsDocumentLoaded && htmlNavigationService.GetLoggedInStatus() is LoggedInStatus.LoggedOut)
        {
            return;
        }

        Dictionary<string, string> content = new()
        {
            { "authenticity_token", htmlNavigationService.GetLogoutFormAuthenticityToken() },
            { "_method", "delete" }
        };

        HttpResponseMessage postResponse = await httpClient.PostAsync(urlOptions.Value.LogOutUrl, new FormUrlEncodedContent(content));
        while (postResponse.StatusCode is HttpStatusCode.Moved) // Should Be RedirectKeepVerb (307), but Cloudflare returns 301.
        {
            postResponse = await httpClient.PostAsync(postResponse.Headers.Location, new FormUrlEncodedContent(content));
        }

        while (postResponse.StatusCode is HttpStatusCode.Redirect)
        {
            postResponse = await httpClient.GetAsync(postResponse.Headers.Location);
        }

        if (!postResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Logging out. {StatusCode} {Reason}", postResponse.StatusCode, postResponse.ReasonPhrase);
        }

        // Check if we logged out successfully or not.
        htmlNavigationService.LoadDocument(await postResponse.Content.ReadAsStreamAsync());
        if (htmlNavigationService.GetLoggedInStatus() is LoggedInStatus.LoggedOut)
        {
            logger.LogInformation("Successfully logged out.");
        }
        else
        {
            logger.LogError("Error Logging out.");
        }
    }
}
