using HtmlAgilityPack;

namespace AO3Statistics.ExtensionMethods;

/// <summary>
/// Provides extension methods for the <see cref="HtmlNode"/> class.
/// </summary>
public static class HtmlNodeExtensions
{
    /// <summary>
    /// <inheritdoc cref="HtmlNode.SelectSingleNode(string)"/>
    /// </summary>
    /// <param name="node">The current HtmlNode instance.</param>
    /// <param name="xPath">The XPath expression.</param>
    /// <param name="_">A placeholder parameter to differentiate this method from the original SelectSingleNode method.</param>
    /// <returns><inheritdoc cref="HtmlNode.SelectSingleNode(string)"/></returns>
    public static HtmlNode? SelectSingleNode(this HtmlNode node, string xPath, bool _) => node.SelectSingleNode(xPath);
}
