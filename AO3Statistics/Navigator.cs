using System.Globalization;
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
    /// Verified working on 16.11.2022
    /// </remarks>
    public const string XPathMultiChapter = "//body/div[@class='wrapper']/div[@class='wrapper']/div[@class='chapters-show region']/div[@class='work']/div[@class='wrapper']/dl/dd[@class='stats']/dl";

    /// <summary>
    /// HTML XPath for the statistics of a single-chapter fic on AO3.
    /// </summary>
    /// <remarks>
    /// Verified working on 16.11.2022
    /// </remarks>
    public const string XPathSingleChapter = "//body/div[@class='wrapper']/div[@class='wrapper']/div[@class='works-show region']/div[@class='wrapper']/dl/dd[@class='stats']/dl";

    /// <summary>
    /// Initializes a new instance of the <see cref="Navigator"/> class and extracts the statistics element from the HTML tree.
    /// </summary>
    /// <param name="uri">The web address to fetch statistics from.</param>
    /// <param name="xPath">
    /// The XPath pointing to the statistics element.<br/>
    /// See <see cref="XPathMultiChapter"/> and <see cref="XPathSingleChapter"/>
    /// </param>
    /// <exception cref="NavigatorException"></exception>
    public Navigator(Uri uri, string? xPath)
    {
        _statisticsContainerNode = NavigateToNode(new HtmlWeb().Load(uri, "GET"), xPath);

        if (_statisticsContainerNode is null)
        {
            throw new NavigatorException("Failed to navigate Node tree. (AO3 could have changed page format.)", xPath, true);
        }
    }

    private static HtmlNode? NavigateToNode(HtmlDocument document, string? xPath)
    {
        try
        {
            return document.DocumentNode.SelectSingleNode(xPath);
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
    /// Gets the specified stat.
    /// </summary>
    /// <param name="statType"> Type of the stat to look up.</param>
    /// <remarks>Return success behaves differently between <see cref="StatTypes"/>.<br/> i.e. Kudos will never fail, since they aren't in the tree if a fic has 0 kudos. </remarks>
    /// <returns>
    /// A <see cref="Boolean"/> which, if <see langword="true"/>, the <see cref="Int32"/> value associated.<br/>
    /// or if <see langword="false"/> a 0.
    /// </returns>
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

        return (Int32.TryParse(intString, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int result), result);
    }
}
