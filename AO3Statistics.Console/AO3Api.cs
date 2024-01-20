using System.Net;

using AO3Statistics.ConsoleApp.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics.ConsoleApp;
public class AO3Api(
    ILogger<AO3Api> logger,
    IOptions<UrlOptions> urlOptions,
    HtmlNavigator htmlNavigator,
    HttpClient httpClient,
    LoginManager loginManager)
{
    private readonly ILogger<AO3Api> logger = logger;
    private readonly HtmlNavigator htmlNavigator = htmlNavigator;
    private readonly HttpClient httpClient = httpClient;
    private readonly LoginManager loginManager = loginManager;
    private readonly IOptions<UrlOptions> urlOptions = urlOptions;

    /// <summary>
    /// Log in to AO3.
    /// </summary>
    /// <returns>Returns <see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException"> Thrown when login is attempted while already logged in.</exception>
    public Task<bool> LoginAsync() => loginManager.LoginAsync();

    /// <summary>
    /// Logs out of AO3.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when logout is attempted while logged out.</exception>
    public Task LogoutAsync() => loginManager.LogoutAsync();

    public async Task<StatisticsSnapshotModel?> GetStatisticsSnapshotAsync()
    {
        // HTTP GET the statistics page.
        HttpResponseMessage getResponse = await httpClient.GetAsync(urlOptions.Value.StatsUrl);
        if (getResponse.StatusCode is HttpStatusCode.Moved)
        {
            getResponse = await httpClient.GetAsync(getResponse.Headers.Location);
        }

        if (!getResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error getting to the statistics page. {StatusCode} {Reason}", getResponse.StatusCode, getResponse.ReasonPhrase);
            return null;
        }

        htmlNavigator.LoadDocument(await getResponse.Content.ReadAsStreamAsync());

        UserStatisticsModel? userStatistics = htmlNavigator.GetUserStatistics();
        if (userStatistics is null)
        {
            logger.LogError("Failure to parse user statistics.");
            return null;
        }

        var workStatistics = htmlNavigator.GetWorkStatistics();

        if (workStatistics.IsSuccess is false)
        {
            logger.LogError("Error parsing work statistics.");
            return null;
        }

        StatisticsSnapshotModel statisticsSnapshot = new(DateOnly.FromDateTime(DateTime.Now))
        {
            UserStatistics = userStatistics,
            WorkStatistics = workStatistics.WorkStatistics
        };

        return statisticsSnapshot;
    }
}
