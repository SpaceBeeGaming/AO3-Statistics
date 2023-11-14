using System.Net;

using AO3Statistics.ConsoleApp.Enums;
using AO3Statistics.ConsoleApp.ExtensionMethods;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics.ConsoleApp;
public class LoginManager(HtmlNavigator htmlNavigator,
    IOptions<UserOptions> userOptions,
    IOptions<UrlOptions> urlOptions,
    ILogger<LoginManager> logger,
    HttpClient httpClient)
{
    private readonly HtmlNavigator htmlNavigator = htmlNavigator;
    private readonly UserOptions userOptions = userOptions.Value;
    private readonly UrlOptions urlOptions = urlOptions.Value;
    private readonly ILogger<LoginManager> logger = logger;
    private readonly HttpClient httpClient = httpClient;

    /// <summary>
    /// Log in to AO3.
    /// </summary>
    /// <returns>Returns <see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException"> Thrown when login is attempted while already logged in.</exception>
    public async Task<bool> LoginAsync()
    {
        // Check that we have a username and a password.
        if (userOptions.Username.IsNullOrWhitespace() || userOptions.Password.IsNullOrWhitespace())
        {
            logger.LogError("Username or password is missing.");
            return false;
        }

        // HTTP GET the login page.
        HttpResponseMessage getResponse = await httpClient.GetAsync(urlOptions.LoginUrl);
        if (getResponse.StatusCode is HttpStatusCode.Moved)
        {
            getResponse = await httpClient.GetAsync(getResponse.Headers.Location);
        }

        if (!getResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Getting to the Login page. {StatusCode} {Reason}", getResponse.StatusCode, getResponse.ReasonPhrase);
            return false;
        }

        // Construct the content for the login attempt.
        htmlNavigator.LoadDocument(await getResponse.Content.ReadAsStreamAsync());

        // Check if we're already logged in. No need to try to do it twice.
        // This condition should never be met unless this program has a bug.
        if (htmlNavigator.IsDocumentLoaded && htmlNavigator.GetLoggedInStatus() is LoggedInStatus.LoggedId)
        {
            logger.LogInformation("Already logged in.");
            return true;
        }

        Dictionary<string, string> content = new()
        {
            { "authenticity_token", htmlNavigator.GetLoginFormAuthenticityToken() },
            { "user[login]", userOptions.Username },
            { "user[password]", userOptions.Password }
        };

        // HTTP POST the credentials.
        HttpResponseMessage postResponse = await httpClient.PostAsync(urlOptions.LoginUrl, new FormUrlEncodedContent(content));
        if (postResponse.StatusCode is HttpStatusCode.Moved)
        {
            postResponse = await httpClient.PostAsync(postResponse.Headers.Location, new FormUrlEncodedContent(content));
        }

        if (postResponse.StatusCode is HttpStatusCode.Redirect)
        {
            postResponse = await httpClient.GetAsync(postResponse.Headers.Location);
        }

        if (!postResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Logging in. {StatusCode} {Reason}", postResponse.StatusCode, postResponse.ReasonPhrase);
            return false;
        }

        // Check if we logged in successfully or not.
        htmlNavigator.LoadDocument(await postResponse.Content.ReadAsStreamAsync());
        if (htmlNavigator.GetLoggedInStatus() is LoggedInStatus.LoggedId)
        {
            logger.LogInformation("Successfully logged in as {Username}", userOptions.Username);
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
        HttpResponseMessage getResponse = await httpClient.GetAsync(urlOptions.LogOutUrl);
        if (getResponse.StatusCode is HttpStatusCode.Moved)
        {
            getResponse = await httpClient.GetAsync(getResponse.Headers.Location);
        }

        if (!getResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Getting to the Logout page. {StatusCode} {Reason}", getResponse.StatusCode, getResponse.ReasonPhrase);
        }

        // Construct the content for the logout attempt.
        htmlNavigator.LoadDocument(await getResponse.Content.ReadAsStreamAsync());

        // Check if we're already logged out. No need to try to do it twice.
        // This condition should never be met unless this program has a bug.
        if (htmlNavigator.IsDocumentLoaded && htmlNavigator.GetLoggedInStatus() is LoggedInStatus.LoggedOut)
        {
            return;
        }

        Dictionary<string, string> content = new()
        {
            { "authenticity_token", htmlNavigator.GetLogoutFormAuthenticityToken() },
            { "_method", "delete" }
        };

        HttpResponseMessage postResponse = await httpClient.PostAsync(urlOptions.LogOutUrl, new FormUrlEncodedContent(content));
        if (postResponse.StatusCode is HttpStatusCode.Moved)
        {
            postResponse = await httpClient.PostAsync(postResponse.Headers.Location, new FormUrlEncodedContent(content));
        }

        if (postResponse.StatusCode is HttpStatusCode.Redirect)
        {
            postResponse = await httpClient.GetAsync(postResponse.Headers.Location);
        }

        if (postResponse.StatusCode is HttpStatusCode.Redirect)
        {
            postResponse = await httpClient.GetAsync(postResponse.Headers.Location);
        }

        if (!postResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error Logging out. {StatusCode} {Reason}", postResponse.StatusCode, postResponse.ReasonPhrase);
        }

        // Check if we logged out successfully or not.
        htmlNavigator.LoadDocument(await postResponse.Content.ReadAsStreamAsync());
        if (htmlNavigator.GetLoggedInStatus() is LoggedInStatus.LoggedOut)
        {
            logger.LogInformation("Successfully logged out.");
        }
        else
        {
            logger.LogError("Error Logging out.");
        }
    }
}
