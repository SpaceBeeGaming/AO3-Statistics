using System.Xml.XPath;

using HtmlAgilityPack;

namespace AO3Statistics;

/// <summary>
/// Facilitates fetching and extraction of statistic data from AO3 fic HTML tree.
/// </summary>
public sealed class Navigator
{
    private readonly HtmlNode? _statisticsContainerNode;

    /// <summary>
    /// HTML XPath for the statistics of a multi-chapter fic on AO3.
    /// </summary>
    /// <remarks>
    /// Verified working on 1.6.2022
    /// </remarks>
    public const string PathMultiChapter = "//body/div[@class='wrapper']/div[@class='wrapper']/div[@class='chapters-show region']/div[@class='work']/div[@class='wrapper']/dl/dd[@class='stats']/dl";

    /// <summary>
    /// HTML XPath for the statistics of a single-chapter fic on AO3.
    /// </summary>
    /// <remarks>
    /// Verified working on 1.6.2022
    /// </remarks>
    public const string PathSingleChapter = "//body/div[@class='wrapper']/div[@class='wrapper']/div[@class='works-show region']/div[@class='wrapper']/dl/dd[@class='stats']/dl";

    /// <summary>
    /// Initializes a new instance of the <see cref="Navigator"/> class and extracts the statistics element from the HTML tree.
    /// </summary>
    /// <param name="uri">The web address to fetch statistics from.</param>
    /// <param name="path">
    /// The XPath pointing to the statistics element.<br/>
    /// See <see cref="PathMultiChapter"/> and <see cref="PathSingleChapter"/>
    /// </param>
    /// <exception cref="NavigatorException"></exception>
    public Navigator(Uri uri, string? path)
    {
        _statisticsContainerNode = NavigateToNode(new HtmlWeb().Load(uri, "GET"), path);

        if (_statisticsContainerNode is null)
        {
            throw new NavigatorException("Failed to navigate Node tree. (AO3 could have changed page format.)", path, true);
        }
    }

    private static HtmlNode? NavigateToNode(HtmlDocument document, string? path)
    {
        try
        {
            return document.DocumentNode.SelectSingleNode(path);
        }
        catch (XPathException ex)
        {
            throw new NavigatorException("Parsing the path failed, most likely a syntax error.", ex);
        }
    }

    /// <summary>
    /// Returns all the children of the selected node. Useful when constructing a custom path.
    /// </summary>
    /// <param name="node"><see cref="HtmlNode"/> to index.</param>
    /// <returns>the children of the node.</returns>
    public static IEnumerable<string> ListChildren(HtmlNode node) =>
        node.ChildNodes.Select(child => $"{child.Name}; {child.GetClasses().FirstOrDefault()}").ToList();

    /// <summary>
    /// Gets the specified stat from the <see cref="HtmlNode"/> obtained from <see cref="NavigateToNode(HtmlDocument, string?)"/>.
    /// </summary>
    /// <param name="statType"> Name of the stat to look up.</param>
    /// <returns>The value of the stat as <see cref="String"/> or <see cref="String.Empty"/> if not found.</returns>
    public (bool IsSuccess, int value) GetValue(StatTypes statType)
    {
        HtmlNode? statisticNode = _statisticsContainerNode!.SelectSingleNode($"./dd[@class='{statType.ToString().ToLowerInvariant()}']");
        string? intString = statType switch
        {
            StatTypes.Hits or
            StatTypes.Words => statisticNode?.InnerText,
            StatTypes.Kudos => statisticNode is not null ? statisticNode.InnerText : "0",
            StatTypes.Chapters => statisticNode?.InnerText.Split('/')[0],
            _ => throw new NotImplementedException(),
        };

        return (Int32.TryParse(intString, out int result), result);
    }
}
