using HtmlAgilityPack;

namespace AO3Statistics.ExtensionMethods;

/// <summary>
/// Extension methods for <see cref="HtmlNode"/>.
/// </summary>
public static class HtmlNodeExtensions
{

    /// <summary>
    /// <inheritdoc cref="HtmlNode.SelectSingleNode(string)"/>
    /// </summary>
    /// <remarks>This method has nullable annotations.</remarks>
    /// <param name="node">The node to start from.</param>
    /// <param name="xPath"><inheritdoc cref="HtmlNode.SelectSingleNode(string)"/></param>
    /// <param name="_">A dummy to differentiate from the original.</param>
    /// <returns><inheritdoc cref="HtmlNode.SelectSingleNode(string)"/></returns>
    public static HtmlNode? SelectSingleNode(this HtmlNode node, string xPath, bool _) => node.SelectSingleNode(xPath);
}
