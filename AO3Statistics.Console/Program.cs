using System.Net;
using System.Net.Http.Headers;

using AO3Statistics.ConsoleApp;

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

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<UserOptions>().Bind(context.Configuration.GetSection(nameof(UserOptions)));
        services.AddOptions<XPathOptions>().Bind(context.Configuration.GetSection(nameof(XPathOptions)));
        services.AddOptions<UrlOptions>().Bind(context.Configuration.GetSection(nameof(UrlOptions)))
            .PostConfigure<IOptions<UserOptions>>((options, userOptions) =>
                options.StatsUrl = new Uri(options.StatsUrl.OriginalString.Replace("<USERNAME>", userOptions.Value.Username)));

        services.AddHostedService<ConsoleHostedService>();
        services.AddSingleton(httpClient);
        services.AddSingleton<LoginManager>();
        services.AddSingleton<AO3Api>();
        services.AddSingleton<HtmlNavigator>();

    }).RunConsoleAsync();