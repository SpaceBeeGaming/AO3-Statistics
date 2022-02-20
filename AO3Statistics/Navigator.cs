using System.Xml.XPath;

using HtmlAgilityPack;

namespace AO3Statistics;

/// <summary>
/// Provides Methods to extract statistic data from AO3 HTML tree.
/// </summary>
public class Navigator
{
    private readonly HtmlNode? _statisticsContainerNode;

    /// <summary>
    /// The default path used by <see cref="NavigateToNode(HtmlDocument, string?)"/>.
    /// </summary>
    public const string DefaultPathMultiChapter = "//body/div[@class='wrapper']/div[@class='wrapper']/div[@class='chapters-show region']/div[@class='work']/div[@class='wrapper']/dl/dd[@class='stats']/dl";
    public const string DefaultPathSingleChapter = "//body/div[@class='wrapper']/div[@class='wrapper']/div[@class='works-show region']/div[@class='wrapper']/dl/dd[@class='stats']/dl";

    /// <summary>
    /// Initializes a new instance of the <see cref="Navigator"/> class.
    /// </summary>
    /// <param name="uri">The web address to fetch statistics from.</param>
    /// <param name="path">
    /// Optional path to navigate to. Defaults to <see cref="DefaultPathMultiChapter"/>.
    /// Override when the default isn't working for some reason.
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
    /// <param name="name"> Name of the stat to look up.</param>
    /// <returns>The value of the stat as <see cref="String"/> or <see cref="String.Empty"/> if not found.</returns>
    public (bool IsSuccess, int value) GetValue(StatTypes name)
    {
        HtmlNode? statisticNode = _statisticsContainerNode!.SelectSingleNode($"./dd[@class='{name.ToString().ToLowerInvariant()}']");
        var intString = name switch
        {
            StatTypes.Kudos => statisticNode is not null ? statisticNode.InnerText : 0,
            StatTypes.Hits or
            StatTypes.Words => statisticNode is not null ? statisticNode.InnerText : null,
            StatTypes.Chapters => statisticNode is not null ? statisticNode.InnerText.Split('/')[0] : null,
            _ => throw new NotImplementedException(),
        };

        return (Int32.TryParse(intString, out int result), result);
    }
}
