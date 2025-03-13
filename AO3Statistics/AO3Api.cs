using AO3Statistics.Models;
using AO3Statistics.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics;
public class AO3Api(
    ILogger<AO3Api> logger,
    IOptions<UrlOptions> urlOptions,
    HtmlNavigationService htmlNavigationService,
    HttpHelper httpHelper,
    LoginService loginService)
{
    private readonly ILogger<AO3Api> logger = logger;
    private readonly HtmlNavigationService htmlNavigationService = htmlNavigationService;
    private readonly HttpHelper httpHelper = httpHelper;
    private readonly LoginService loginService = loginService;
    private readonly IOptions<UrlOptions> urlOptions = urlOptions;

    /// <summary>
    /// Log in to AO3.
    /// </summary>
    /// <returns>Returns <see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException"> Thrown when login is attempted while already logged in.</exception>
    public Task<bool> LoginAsync() => loginService.LoginAsync();

    /// <summary>
    /// Logs out of AO3.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when logout is attempted while logged out.</exception>
    public Task LogoutAsync() => loginService.LogoutAsync();

    public async Task<StatisticsSnapshotModel?> GetStatisticsSnapshotAsync()
    {
        // HTTP GET the statistics page.
        logger.LogInformation("GET Statistics page");
        HttpResponseMessage getResponse = await httpHelper.RedirectGet(urlOptions.Value.StatsUrl);

        if (!getResponse.IsSuccessStatusCode)
        {
            logger.LogError("Error getting to the statistics page. {StatusCode} {Reason}", getResponse.StatusCode, getResponse.ReasonPhrase);
            return null;
        }

        htmlNavigationService.LoadDocument(await getResponse.Content.ReadAsStreamAsync());

        UserStatisticsModel? userStatistics = htmlNavigationService.GetUserStatistics();
        if (userStatistics is null)
        {
            logger.LogError("Failure to parse user statistics.");
            return null;
        }

        (bool IsSuccess, List<WorkStatisticsModel> WorkStatistics) workStatistics = htmlNavigationService.GetWorkStatistics();

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
