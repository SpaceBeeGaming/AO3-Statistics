using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

using AO3Statistics.ConsoleApp.Enums;
using AO3Statistics.ConsoleApp.Exceptions;
using AO3Statistics.ConsoleApp.ExtensionMethods;
using AO3Statistics.ConsoleApp.Models;

using HtmlAgilityPack;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics.ConsoleApp.Services;

/// <summary>
/// Provides methods to extract data from <see cref="HtmlDocument"/>.
/// </summary>
public sealed class HtmlNavigator(
    ILogger<HtmlNavigator> logger,
    IOptions<UserOptions> userOptions,
    IOptions<XPathOptions> xPathOptions)
{
    private readonly ILogger<HtmlNavigator> logger = logger;
    private readonly IOptions<UserOptions> userOptions = userOptions;
    private readonly IOptions<XPathOptions> xPathOptions = xPathOptions;
    private HtmlDocument? _document;

    public bool IsDocumentLoaded => _document is not null;

    /// <summary>
    /// Loads the <see cref="HtmlDocument"/> from the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to load the <see cref="HtmlDocument"/> from.</param>
    public void LoadDocument(Stream stream)
    {
        _document ??= new HtmlDocument();
        _document.Load(stream);
    }

    /// <summary>
    /// Determines if the currently loaded document is in logged in state.
    /// </summary>
    /// <returns>Returns whether we're logged in or not.</returns>
    /// <exception cref="UnreachableException">Thrown when developer did a dummy.</exception>
    public LoggedInStatus GetLoggedInStatus()
    {
        ThrowIfDocumentNull(_document);

        HtmlNode node = SelectSingleNodeFromRoot(_document, "//body");
        string classString = node.GetClasses().FirstOrDefault(x => x.StartsWith("logged-"), string.Empty);
        return classString switch
        {
            "logged-in" => LoggedInStatus.LoggedId,
            "logged-out" => LoggedInStatus.LoggedOut,
            _ => throw new UnreachableException("AO3 html body always has a login state class. Thus the default branch will never match."),
        };
    }

    /// <summary>
    /// Gets the authenticity token from the login from.
    /// </summary>
    /// <returns>The authenticity token.</returns>
    /// <exception cref="HtmlNavigatorException">Thrown when the token cannot be found for whatever reason.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="IsDocumentLoaded"/> is <see langword="false"/>.</exception>
    public string GetLoginFormAuthenticityToken() => GetAuthenticityToken(xPathOptions.Value.LoginFormAuthenticityTokenXPath);

    /// <summary>
    /// Gets the authenticity token from the logout from.
    /// </summary>
    /// <returns>The authenticity token.</returns>
    /// <exception cref=" HtmlNavigatorException">Thrown when the token cannot be found for whatever reason.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="IsDocumentLoaded"/> is <see langword="false"/>.</exception>

    public string GetLogoutFormAuthenticityToken() => GetAuthenticityToken(xPathOptions.Value.LogoutFormAuthenticityTokenXPath);

    /// <summary>
    /// Extracts the user statistics from the loaded document.
    /// </summary>
    /// <returns>The user statistics.</returns>
    public UserStatisticsModel? GetUserStatistics()
    {
        var userSubscriptions = GetUserStatisticValue(UserStatisticTypes.UserSubscriptions);
        var kudos = GetUserStatisticValue(UserStatisticTypes.Kudos);
        var commentThreads = GetUserStatisticValue(UserStatisticTypes.CommentThreads);
        var bookmarks = GetUserStatisticValue(UserStatisticTypes.Bookmarks);
        var subscriptions = GetUserStatisticValue(UserStatisticTypes.Subscriptions);
        var wordCount = GetUserStatisticValue(UserStatisticTypes.WordCount);
        var hits = GetUserStatisticValue(UserStatisticTypes.Hits);

        Dictionary<UserStatisticTypes, bool> userStatistics = new()
        {
            { UserStatisticTypes.UserSubscriptions, userSubscriptions.IsSuccess },
            { UserStatisticTypes.Kudos, kudos.IsSuccess },
            { UserStatisticTypes.CommentThreads, commentThreads.IsSuccess },
            { UserStatisticTypes.Bookmarks, bookmarks.IsSuccess },
            { UserStatisticTypes.Subscriptions, subscriptions.IsSuccess },
            { UserStatisticTypes.WordCount, wordCount.IsSuccess },
            { UserStatisticTypes.Hits, hits.IsSuccess }
        };

        if (HasParsingErrors(userStatistics))
        {
            return null;
        }

        UserStatisticsModel userStatisticsModel = new()
        {
            Username = userOptions.Value.Username,
            UserSubscriptions = userSubscriptions.value,
            Kudos = kudos.value,
            CommentThreads = commentThreads.value,
            Bookmarks = bookmarks.value,
            Subscriptions = subscriptions.value,
            WordCount = wordCount.value,
            Hits = hits.value,
        };

        return userStatisticsModel;

    }
    /// <summary>
    /// Extracts the <see cref="UserStatisticsModel"/> from the loaded document.
    /// </summary>
    /// <returns>A list of work statistics.</returns>
    public (bool IsSuccess, List<WorkStatisticsModel> WorkStatistics) GetWorkStatistics()
    {
        ThrowIfDocumentNull(_document);

        bool hasErrors = false;
        List<WorkStatisticsModel> workStatistics = [];
        List<HtmlNode> workNodes = GetWorkNodes();
        foreach (HtmlNode statisticNode in workNodes)
        {
            Dictionary<WorkStatisticTypes, bool> workStatisticsDict = [];

            // Work ID
            string? workId = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.WorkId);
            logger.LogDebug("Work ID: {WorkId}", workId);
            workStatisticsDict.Add(WorkStatisticTypes.WorkId, workId.IsNotNullOrWhitespace());

            // Work name
            string? workName = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.WorkName);
            logger.LogDebug("Work name: {WorkName}", workName);
            workStatisticsDict.Add(WorkStatisticTypes.WorkName, workName.IsNotNullOrWhitespace());

            // Fandom names
            string? fandomName = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.FandomName);
            logger.LogDebug("Fandom name: {FandomName}", fandomName);
            workStatisticsDict.Add(WorkStatisticTypes.FandomName, fandomName.IsNotNullOrWhitespace());

            // Word count
            string? wordCountString = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.WordCount);
            logger.LogDebug("Word count: {WordCount}", wordCountString);
            workStatisticsDict.Add(WorkStatisticTypes.WordCount, int.TryParse(wordCountString, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int wordCount));

            // Subscriptions
            string? subscriptionsString = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.Subscriptions);
            logger.LogDebug("Subscriptions: {Subscriptions}", subscriptionsString);
            workStatisticsDict.Add(WorkStatisticTypes.Subscriptions, int.TryParse(subscriptionsString, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int subscriptions));

            // Hits
            string? hitsString = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.Hits);
            logger.LogDebug("Hits: {Hits}", hitsString);
            workStatisticsDict.Add(WorkStatisticTypes.Hits, int.TryParse(hitsString, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int hits));

            // Kudos
            string? kudosSting = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.Kudos);
            logger.LogDebug("Kudos: {Kudos}", kudosSting);
            workStatisticsDict.Add(WorkStatisticTypes.Kudos, int.TryParse(kudosSting, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int kudos));

            // Comment threads
            string? commentThreadsString = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.CommentThreads);
            logger.LogDebug("Comment threads: {Comment threads}", commentThreadsString);
            workStatisticsDict.Add(WorkStatisticTypes.CommentThreads, int.TryParse(commentThreadsString, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int commentThreads));

            // Bookmarks
            string? bookmarksString = GetWorkStatisticValue(statisticNode, WorkStatisticTypes.Bookmarks);
            logger.LogDebug("Bookmarks: {Bookmarks}", bookmarksString);
            workStatisticsDict.Add(WorkStatisticTypes.Bookmarks, int.TryParse(bookmarksString, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int bookmarks));

            // Check for failed conversions and log them.
            if (HasParsingErrors(workStatisticsDict, workName))
            {
                hasErrors = true;
                continue;
            }

            workStatistics.Add(new()
            {
                WorkId = workId!,
                WorkName = workName!,
                FandomName = fandomName!,
                WordCount = wordCount,
                Subscriptions = subscriptions,
                Hits = hits,
                Kudos = kudos,
                CommentThreads = commentThreads,
                Bookmarks = bookmarks
            });

            logger.LogInformation("""Work: "{WorkName}" processed.""", workName);
        }

        if (workStatistics.Count != workNodes.Count)
        {
            logger.LogWarning("Failed to parse {FailCount} out of {TotalCount} works.", workNodes.Count - workStatistics.Count, workNodes.Count);
        }

        return (!hasErrors, workStatistics);
    }

    private (bool IsSuccess, int value) GetUserStatisticValue(UserStatisticTypes statisticType)
    {
        ThrowIfDocumentNull(_document);

        HtmlNode? statisticNode = SelectSingleNodeFromRoot(_document, xPathOptions.Value.UserStatisticsXPath)
            .SelectSingleNode($"./dd[@class='{statisticType.ToString(true)}']", true);

        return (int.TryParse(statisticNode?.InnerText, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int result), result);
    }

    private static string? GetWorkStatisticValue(HtmlNode workNode, WorkStatisticTypes statisticType)
    {
        switch (statisticType)
        {
            case WorkStatisticTypes.WorkId:
                return workNode.SelectSingleNode("./dt/a", true)?.GetAttributeValue("href", null)[7..];
            case WorkStatisticTypes.WorkName:
                return WebUtility.HtmlDecode(workNode.SelectSingleNode("./dt/a", true)?.InnerText);
            case WorkStatisticTypes.FandomName:
                return WebUtility.HtmlDecode(workNode.SelectSingleNode($"./dt/span[@class='{statisticType.ToString(true)}']", true)?.InnerText[1..^1]);
            case WorkStatisticTypes.WordCount:
                return workNode.SelectSingleNode($"./dt/span[@class='{statisticType.ToString(true)}']", true)?.InnerText[1..^7];
            case WorkStatisticTypes.Subscriptions:
                HtmlNode? subscriptionsNode = workNode.SelectSingleNode($"./dd/dl[@class='stats']/dd[@class='{statisticType.ToString(true)}']", true);
                return subscriptionsNode is not null ? subscriptionsNode.InnerText : "0"; // Null is expected here when the work has 0 subscriptions.
            case WorkStatisticTypes.Hits:
            case WorkStatisticTypes.Kudos:
            case WorkStatisticTypes.CommentThreads:
            case WorkStatisticTypes.Bookmarks:
                return workNode.SelectSingleNode($"./dd/dl[@class='stats']/dd[@class='{statisticType.ToString(true)}']", true)?.InnerText;
            default:
                throw new UnreachableException("Undefined enum value.");
        }
    }

    private List<HtmlNode> GetWorkNodes()
    {
        ThrowIfDocumentNull(_document);

        HtmlNode workStatisticsNode = SelectSingleNodeFromRoot(_document, xPathOptions.Value.WorkStatisticsXPath);

        List<HtmlNode> workNodes = [];
        foreach (HtmlNode child in workStatisticsNode.ChildNodes.Where(x => x.Name is "li"))
        {
            HtmlNode node = child.SelectSingleNode("./dl", true)
                 ?? throw new HtmlNavigatorException("Specified node wasn't found in the document.", $"{child.XPath}./dl", _document);

            workNodes.Add(node);
        }

        logger.LogInformation("Found {Count} works.", workNodes.Count);

        return workNodes;
    }

    /// <summary>
    /// Extracts the authenticity token from the provided <paramref name="authenticityTokenXPath"/>.
    /// </summary>
    /// <param name="authenticityTokenXPath">The Xpath where the authenticity token can be found.</param>
    /// <returns>The authenticity token</returns>
    /// <exception cref="HtmlNavigatorException">Thrown when the token cannot be found for whatever reason.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="IsDocumentLoaded"/> is <see langword="false"/>.</exception>
    private string GetAuthenticityToken(string authenticityTokenXPath)
    {
        ThrowIfDocumentNull(_document);

        HtmlNode node = SelectSingleNodeFromRoot(_document, authenticityTokenXPath);
        string nameAttribute = node.GetAttributeValue("name", null)
            ?? throw new HtmlNavigatorException("Node doesn't contain attribute: 'name'", node);
        return nameAttribute switch
        {
            "authenticity_token" => node.GetAttributeValue("value", null)
                ?? throw new HtmlNavigatorException("Node doesn't contain attribute: 'value'", node),
            _ => throw new HtmlNavigatorException($"Unexpected value for attribute: 'name' ({nameAttribute})", node)
        };
    }

    /// <summary>
    /// Throws if provided <see cref="HtmlDocument"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="document">The <see cref="HtmlDocument"/> to check for <see langword="null"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown when provided <see cref="HtmlDocument"/> is <see langword="null"/>.</exception>
    private static void ThrowIfDocumentNull([NotNull] HtmlDocument? document) =>
        _ = document ?? throw new InvalidOperationException("Document not loaded. Call LoadDocument() first.");

    /// <summary>
    /// Navigates to the <see cref="HtmlNode"/> described by <paramref name="xPath"/> from the <paramref name="document"/>.
    /// </summary>
    /// <param name="document">The <see cref="HtmlDocument"/> to search from.</param>
    /// <param name="xPath">The XPath to search.</param>
    /// <returns>The <see cref="HtmlNode"/> if found, otherwise throws a <see cref="HtmlNavigatorException"/>.</returns>
    /// <exception cref="HtmlNavigatorException">Thrown if the <paramref name="document"/>doesn't contain the specified node.</exception>
    private static HtmlNode SelectSingleNodeFromRoot(HtmlDocument document, string xPath)
    {
        return document.DocumentNode.SelectSingleNode(xPath)
            ?? throw new HtmlNavigatorException("Specified node wasn't found in the document.", xPath, document);
    }

    /// <summary>
    /// Checks and logs if the dictionary has recorded parsing errors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="statistics"></param>
    /// <returns>True if errors exist</returns>
    private bool HasParsingErrors<T>(Dictionary<T, bool> statistics, string? workName = "<>") where T : notnull
    {
        if (statistics.Any(x => x.Value is not true))
        {
            foreach (var statistic in statistics.Where(x => x.Value is not true))
            {
                switch (workName)
                {
                    case "<>":
                        logger.LogWarning("""Error parsing {statisticType} on user statistics.""", statistic.Key);
                        break;
                    case null:
                        logger.LogWarning("""Error parsing {statisticType} on unknown work.""", statistic.Key);
                        break;
                    default:
                        logger.LogWarning("""Error parsing {statisticType} on work "{WorkName}".""", statistic.Key, workName);
                        break;
                }
            }

            return true;
        }

        return false;
    }
}
