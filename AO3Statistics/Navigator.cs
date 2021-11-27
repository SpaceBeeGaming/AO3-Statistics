using System;
using System.Xml.XPath;

using HtmlAgilityPack;

namespace AO3Statistics;

/// <summary>
/// Provides Methods to extract statistic data from AO3 Html tree.
/// </summary>
public class Navigator
{
    private readonly HtmlNode? _statisticsContainerNode;

    /// <summary>
    /// The default path used by <see cref="NavigateToNode(HtmlDocument, string?)"/>.
    /// </summary>
    public const string DefaultPath = "//body/div[@class='wrapper']/div[@class='wrapper']/div[@class='chapters-show region']/div[@class='work']/div[@class='wrapper']/dl/dd[@class='stats']/dl";

    /// <summary>
    /// Initializes a new isntance of the <see cref="Navigator"/> class.
    /// </summary>
    /// <param name="uri">The webaddress to fetch statistics from.</param>
    /// <param name="path">
    /// Optional path to navigate to. Defaults to <see cref="DefaultPath"/>.
    /// Override when the default isn't working for some reason.
    /// </param>
    /// <exception cref="NavigatorException"></exception>
    public Navigator(Uri uri, string? path = DefaultPath)
    {
        _statisticsContainerNode = NavigateToNode(new HtmlWeb().Load(uri), path);

        if (_statisticsContainerNode is null)
        {
            throw new NavigatorException("Failed to navigate Node tree. (AO3 could have changed page format.)", path);
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
    /// Returns all the children of the selected node. Useful when construcxting a custom path.
    /// </summary>
    /// <param name="node"><see cref="HtmlNode"/> to index.</param>
    /// <returns>the children of the node.</returns>
    public static IEnumerable<string> ListChildren(HtmlNode node) =>
        node.ChildNodes.Select(child => $"{child.Name}; {child.GetClasses().FirstOrDefault()}").ToList();

    /// <summary>
    /// Extracts the specified stat from the <see cref="HtmlNode"/> obtained from <see cref="NavigateToNode(HtmlDocument, string?)"/>.
    /// </summary>
    /// <param name="name"> Name of the stat to look up.</param>
    /// <returns>The value of the stat as <see cref="String"/> or <see cref="String.Empty"/> if not found.</returns>
    public string GetValue(StatTypes name)
    {
        HtmlNode? statisticNode = _statisticsContainerNode.SelectSingleNode($"./dd[@class='{name.ToString().ToLowerInvariant()}']");

        return name switch
        {
            StatTypes.Hits or
            StatTypes.Kudos or
            StatTypes.Words =>
                statisticNode is not null ? statisticNode.InnerText : String.Empty,
            StatTypes.Chapters =>
                statisticNode is not null ? statisticNode.InnerText.Split('/')[0] : String.Empty,
            _ =>
                throw new NotImplementedException(),
        };
    }
}
