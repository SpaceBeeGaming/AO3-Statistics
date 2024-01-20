using HtmlAgilityPack;

namespace AO3Statistics.Exceptions;

public sealed class HtmlNavigatorException : Exception
{
    public string? XPath { get; }
    public HtmlNode? Node { get; }
    public HtmlDocument? Document { get; }
    public HtmlNavigatorException() : base()
    {
    }

    public HtmlNavigatorException(string? message) : base(message)
    {
    }

    public HtmlNavigatorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public HtmlNavigatorException(string? message, string? xPath) : base(message)
    {
        XPath = xPath;
    }

    public HtmlNavigatorException(string? message, string? xPath, HtmlDocument? document) : this(message, xPath)
    {
        Document = document;
    }

    public HtmlNavigatorException(string? message, HtmlDocument? document) : base(message)
    {
        Document = document;
    }

    public HtmlNavigatorException(string? message, HtmlNode? node) : base(message)
    {
        Node = node;
    }
}
