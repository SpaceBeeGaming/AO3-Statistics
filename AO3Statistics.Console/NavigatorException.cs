using HtmlAgilityPack;

namespace AO3Statistics.ConsoleApp;

public sealed class NavigatorException : Exception
{
    public string? XPath { get; }
    public HtmlNode? Node { get; }
    public HtmlDocument? Document { get; }
    public NavigatorException() : base()
    {
    }

    public NavigatorException(string? message) : base(message)
    {
    }

    public NavigatorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public NavigatorException(string? message, string? xPath) : base(message)
    {
        XPath = xPath;
    }

    public NavigatorException(string? message, string? xPath, HtmlDocument? document) : this(message, xPath)
    {
        Document = document;
    }

    public NavigatorException(string? message, HtmlDocument? document) : base(message)
    {
        Document = document;
    }

    public NavigatorException(string? message, HtmlNode? node) : base(message)
    {
        Node = node;
    }
}
