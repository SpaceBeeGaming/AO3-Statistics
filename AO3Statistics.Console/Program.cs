using System.Net;
using System.Net.Http.Headers;

using AO3Statistics.ConsoleApp;
using AO3Statistics.ConsoleApp.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    [$"{nameof(UserOptions)}:{nameof(UserOptions.PasswordIsFromCommandLine)}"] = args.Contains("--UserOptions:Password") ? bool.TrueString : bool.FalseString,
});

builder.Services.AddOptionsWithValidateOnStart<UserOptions>()
    .Bind(builder.Configuration.GetSection(nameof(UserOptions)))
    .PostConfigure((options) =>
    {
        if (options.PasswordIsProtected is true && options.PasswordIsFromCommandLine is false)
        {
            options.Password = PasswordEncryptor.UnProtectPassword(options.Password);
        }
    })
    .ValidateDataAnnotations();

builder.Services.AddOptionsWithValidateOnStart<OutputOptions>()
    .Bind(builder.Configuration.GetSection(nameof(OutputOptions)))
    .ValidateDataAnnotations();

builder.Services.AddOptionsWithValidateOnStart<XPathOptions>()
    .Bind(builder.Configuration.GetSection(nameof(XPathOptions)))
    .ValidateDataAnnotations();

builder.Services.AddOptionsWithValidateOnStart<UrlOptions>()
    .Bind(builder.Configuration.GetSection(nameof(UrlOptions)))
    .PostConfigure<IOptions<UserOptions>>((options, userOptions) =>
        options.StatsUrl = new Uri(options.StatsUrl.OriginalString.Replace("<USERNAME>", userOptions.Value.Username)))
    .ValidateDataAnnotations();

builder.Services.AddSingleton(httpClient);
builder.Services.AddSingleton<HtmlNavigator>();
builder.Services.AddSingleton<LoginManager>();
builder.Services.AddSingleton<AO3Api>();
builder.Services.AddHostedService<ConsoleHostedService>();

var host = builder.Build();

try
{
    await host.RunAsync();
}
catch (AggregateException ex)
{
    if (ex.InnerException is OptionsValidationException validationEx)
    {
        Console.WriteLine("Configuration issues:");
        foreach (string failure in validationEx.Failures)
        {
            Console.WriteLine(failure);
        }
    }
}

