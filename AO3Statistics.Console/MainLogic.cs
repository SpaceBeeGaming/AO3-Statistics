using AO3Statistics.ConsoleApp.Models;
using AO3Statistics.ConsoleApp.Services.DataDestinationService;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics.ConsoleApp;
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
        if (userOptions.Value.Password is null or "")
        {
            const string NoPasswordProvidedMessage = """
                            No password was provided.
                            Please run the program through the command line with '--UserOptions:Password "<your password here>"' as an argument.
                            """;
            Console.WriteLine(NoPasswordProvidedMessage);
            return 1;
        }

        if (userOptions.Value.PasswordIsProtected is false && userOptions.Value.PasswordIsFromCommandLine is false)
        {
            const string UnprotectedPasswordWarningMessage = """
                            Using Unprotected password!
                            Please run the program through the command line with '--UserOptions:Password "<your password here>"' as an argument for further instructions.
                            """;
            Console.WriteLine(UnprotectedPasswordWarningMessage);
        }
        else if (userOptions.Value.PasswordIsProtected is false && userOptions.Value.PasswordIsFromCommandLine is true)
        {
            string protectedPassword = PasswordEncryptor.ProtectPassword(userOptions.Value.Password);

            const string ProtectedPasswordInfoMessage = """
                            Password provided through command line. 
                            If you're always running the program manually, you can disable this message by setting "UserOptions:PasswordIsProtected" to true in the appsettings.json file.

                            If you'd like the program to remember your password, please make the following changes to the appsettings.json file. 
                            1. Paste "{0}" to "UserOptions:Password".
                            2. Set "UserOptions:PasswordIsProtected" to true.
                            Important Note: On operating systems other than Windows, the password is NOT encrypted and is trivial to get back in original form.
                            """;
            Console.WriteLine(ProtectedPasswordInfoMessage, protectedPassword);
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