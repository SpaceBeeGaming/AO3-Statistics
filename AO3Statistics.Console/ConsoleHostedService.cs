using AO3Statistics.ConsoleApp.Models;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics.ConsoleApp;
internal class ConsoleHostedService(
    IHostApplicationLifetime applicationLifetime,
    ILogger<ConsoleHostedService> logger,
    AO3Api aO3Api,
    IOptions<UserOptions> userOptions)
    : IHostedService
{
    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime;
    private readonly ILogger<ConsoleHostedService> logger = logger;
    private readonly IOptions<UserOptions> userOptions = userOptions;
    private readonly AO3Api aO3Api = aO3Api;
    private int? _exitCode;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    if (userOptions.Value.Password is null or "")
                    {
                        const string NoPasswordMessage = """
                            No password was provided.
                            Please run the program through the command line with '--UserOptions:Password "<your password here>"' as an argument.
                            """;
                        logger.LogWarning(NoPasswordMessage);
                        return;
                    }

                    if (userOptions.Value.PasswordIsProtected is false && userOptions.Value.PasswordIsFromCommandLine is false)
                    {
                        const string UnprotectedPasswordMessage = """
                            Using Unprotected password!
                            Please run the program through the command line with '--UserOptions:Password "<your password here>"' as an argument for further isntructions.
                            """;
                        logger.LogWarning(UnprotectedPasswordMessage);
                    }
                    else if (userOptions.Value.PasswordIsProtected is false && userOptions.Value.PasswordIsFromCommandLine is true)
                    {
                        string protectedPassword = PasswordEncryptor.ProtectPassword(userOptions.Value.Password);

                        const string ProtectedPasswordInfoMessage = """
                            Password provided through command line. 
                            If you're always running the program manually, you can disable this message by setting "UserOptions:PasswordIsProtected" to true in the appsettings.json file.

                            If you'd like the program to remember your password, please make the following changes to the appsettings.json file. 
                            1. Paste "{protectedPassword}" to "UserOptions:Password".
                            2. Set "UserOptions:PasswordIsProtected" to true.
                            Important Note: On operating systems other than Windows, the password is NOT encrypted and is trivial to get back in original form.
                            """;
                        logger.LogInformation(ProtectedPasswordInfoMessage, protectedPassword);
                    }

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
