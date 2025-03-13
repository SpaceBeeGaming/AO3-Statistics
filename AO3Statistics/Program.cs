using System.Net;
using System.Net.Http.Headers;

using AO3Statistics;
using AO3Statistics.Enums;
using AO3Statistics.Models;
using AO3Statistics.Services;
using AO3Statistics.Services.DataDestinationService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

CookieContainer cookieContainer = new();
HttpClientHandler handler = new()
{
    CookieContainer = cookieContainer,
    AllowAutoRedirect = false,
    UseCookies = true
};

HttpClient httpClient = new(handler);
httpClient.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse("AO3Stats/v2"));
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    [$"{nameof(UserOptions)}:{nameof(UserOptions.PasswordIsFromCommandLine)}"] = args.Contains("--Password") ? bool.TrueString : bool.FalseString,
});

builder.Services.AddOptionsWithValidateOnStart<UserOptions>()
    .Bind(builder.Configuration.GetSection(nameof(UserOptions)))
    .PostConfigure((options) =>
    {
        if (options.PasswordIsProtected is true && options.PasswordIsFromCommandLine is false)
        {
            options.Password = PasswordEncryptor.UnProtectPassword(options.Password);
        }
    });

builder.Services.AddOptionsWithValidateOnStart<OutputOptions>()
    .Bind(builder.Configuration.GetSection(nameof(OutputOptions)));

builder.Services.AddOptionsWithValidateOnStart<XPathOptions>()
    .Bind(builder.Configuration.GetSection(nameof(XPathOptions)));

builder.Services.AddOptionsWithValidateOnStart<UrlOptions>()
    .Bind(builder.Configuration.GetSection(nameof(UrlOptions)))
    .PostConfigure<IOptions<UserOptions>>((options, userOptions) =>
        options.StatsUrl = new Uri(options.StatsUrl.OriginalString.Replace("<USERNAME>", userOptions.Value.Username)));

builder.Services.AddSingleton<IValidateOptions<OutputOptions>, ValidateOutputOptions>();
builder.Services.AddSingleton<IValidateOptions<UrlOptions>, ValidateUrlOptions>();
builder.Services.AddSingleton<IValidateOptions<UserOptions>, ValidateUserOptions>();
builder.Services.AddSingleton<IValidateOptions<XPathOptions>, ValidateXPathOptions>();
builder.Services.AddSingleton(httpClient);
builder.Services.AddSingleton<HttpHelper>();
builder.Services.AddSingleton<HtmlNavigationService>();
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<AO3Api>();
builder.Services.AddSingleton<MainLogic>();
builder.Services.AddKeyedSingleton<IDataDestination, MultiCSVDataDestination>("MultiCSV");

builder.Services.AddSingleton<IDataDestination>(serviceProvider =>
{
    OutputFormats outputFormat = serviceProvider.GetRequiredService<IOptions<OutputOptions>>().Value.OutputFormat;
    return serviceProvider.GetRequiredKeyedService<IDataDestination>(outputFormat.ToString());
});

IHost host = builder.Build();

try
{
    await host.StartAsync();

    int exitCode = await host.Services.GetRequiredService<MainLogic>().Run();

    host.Services.GetRequiredService<ILogger<Program>>().LogInformation("Exiting with exit code: {ExitCode}", exitCode);
    Environment.ExitCode = exitCode;
}
catch (OptionsValidationException ex)
{
    Console.WriteLine("Configuration issues:");
    foreach (string failure in ex.Failures)
    {
        Console.WriteLine(failure);
    }
}

