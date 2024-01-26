using AO3Statistics.ExtensionMethods;
using AO3Statistics.Models;
using AO3Statistics.Services.DataDestinationService;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics;
internal class MainLogic(
    ILogger<MainLogic> logger,
    AO3Api aO3Api,
    IOptions<UserOptions> userOptions,
    IDataDestination dataDestination)
{
    private readonly ILogger<MainLogic> logger = logger;
    private readonly IOptions<UserOptions> userOptions = userOptions;
    private readonly IDataDestination dataDestination = dataDestination;
    private readonly AO3Api aO3Api = aO3Api;

    public async Task<int> Run()
    {
        if (userOptions.Value.Password is null or "" && userOptions.Value.PasswordIsFromCommandLine is false)
        {
            const string NoPasswordProvidedMessage = """
                            No password was provided.
                            Please run the program through the command line with '--Password' as an argument.
                            """;
            await Console.Out.WriteLineAsync(NoPasswordProvidedMessage);
            return 1;
        }

        if (userOptions.Value.PasswordIsProtected is false && userOptions.Value.PasswordIsFromCommandLine is false)
        {
            const string UnprotectedPasswordWarningMessage = """
                            Using Unprotected password!
                            Please run the program through the command line with '--Password' as an argument for further instructions.
                            """;
            await Console.Out.WriteLineAsync(UnprotectedPasswordWarningMessage);
        }
        else if (userOptions.Value.PasswordIsProtected is false && userOptions.Value.PasswordIsFromCommandLine is true)
        {
            await Console.Out.WriteLineAsync("Please provide your password: ");
            string? password = await Console.In.ReadLineAsync();
            if (password.IsNullOrWhitespace())
            {
                await Console.Out.WriteLineAsync("Not a valid password.");
                return 1;
            }

            userOptions.Value.Password = password;

            string protectedPassword = PasswordEncryptor.ProtectPassword(password);

            string ProtectedPasswordInfoMessage = $"""
                            Password provided through command line. 
                            If you're always running the program manually, you can disable this message by setting "UserOptions:PasswordIsProtected" to true in the appsettings.json file.

                            If you'd like the program to remember your password, please make the following changes to the appsettings.json file. 
                            1. Paste "{protectedPassword}" to "UserOptions:Password".
                            2. Set "UserOptions:PasswordIsProtected" to true.
                            3. Run WITHOUT --Password
                            Important Note: On operating systems other than Windows, the password is NOT encrypted and is trivial to get back in original form.
                            """;
            await Console.Out.WriteLineAsync(ProtectedPasswordInfoMessage);
        }

        bool loginSucceeded = await aO3Api.LoginAsync();
        if (loginSucceeded is not true)
        {
            return 1;
        }

        StatisticsSnapshotModel? statisticsSnapshot = await aO3Api.GetStatisticsSnapshotAsync();
        logger.LogInformation("{StatisticsSnapshot}", statisticsSnapshot?.ToString(true));
        await aO3Api.LogoutAsync();

        if (statisticsSnapshot is null)
        {
            return 1;
        }

        dataDestination.SaveData(statisticsSnapshot);
        return 0;
    }
}