using HtmlAgilityPack;

namespace AO3Statistics.ConsoleApp.ExtensionMethods;

public static class HtmlNodeExtensions
{
    public static HtmlNode? SelectSingleNode(this HtmlNode node, string xPath, bool _) => node.SelectSingleNode(xPath);
}
