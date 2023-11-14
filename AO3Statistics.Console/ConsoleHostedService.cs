using AO3Statistics.ConsoleApp.Models;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AO3Statistics.ConsoleApp;
internal class ConsoleHostedService(
    IHostApplicationLifetime applicationLifetime,
    ILogger<ConsoleHostedService> logger,
    AO3Api aO3Api)
    : IHostedService
{
    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;
    private readonly ILogger<ConsoleHostedService> logger = logger;
    private readonly AO3Api aO3Api = aO3Api;
    private int? _exitCode;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting with arguments: {arguments}", string.Join(' ', Environment.GetCommandLineArgs()));

        applicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    if (await aO3Api.LoginAsync())
                    {
                        StatisticsSnapshotModel? statisticsSnapshot = await aO3Api.GetStatisticsSnapshotAsync();
                        logger.LogInformation("{StatistiSnapshot}", statisticsSnapshot?.ToString(true));
                        await aO3Api.LogoutAsync();

                        // TODO: Write to file.
                    }

                    _exitCode = 0;
                }
                catch (Exception ex)
                {

                    logger.LogError(ex, "Unhandled exception!");
                    _exitCode = 1;
                }
                finally
                {
                    applicationLifetime.StopApplication();
                }
            }, cancellationToken);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Exiting with return code: {exitCode}", _exitCode);

        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        return Task.CompletedTask;
    }
}
