using HtmlAgilityPack;

namespace AO3Statistics.Exceptions;

/// <summary>
/// Represents an exception that occurs during HTML navigation using XPath.
/// </summary>
public sealed class HtmlNavigationException : Exception
{
    /// <summary>
    /// Gets the XPath expression that caused the exception.
    /// </summary>
    public string? XPath { get; }

    /// <summary>
    /// Gets the HTML node associated with the exception.
    /// </summary>
    public HtmlNode? Node { get; }

    /// <summary>
    /// Gets the HTML document associated with the exception.
    /// </summary>
    public HtmlDocument? Document { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNavigationException"/> class.
    /// </summary>
    public HtmlNavigationException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNavigationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public HtmlNavigationException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNavigationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public HtmlNavigationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNavigationException"/> class with a specified error message and XPath expression.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="xPath">The XPath expression that caused the exception.</param>
    public HtmlNavigationException(string? message, string? xPath) : base(message) => XPath = xPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNavigationException"/> class with a specified error message, XPath expression, and HTML document.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="xPath">The XPath expression that caused the exception.</param>
    /// <param name="document">The HTML document associated with the exception.</param>
    public HtmlNavigationException(string? message, string? xPath, HtmlDocument? document) : this(message, xPath) => Document = document;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNavigationException"/> class with a specified error message and HTML document.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="document">The HTML document associated with the exception.</param>
    public HtmlNavigationException(string? message, HtmlDocument? document) : base(message) => Document = document;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlNavigationException"/> class with a specified error message and HTML node.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="node">The HTML node associated with the exception.</param>
    public HtmlNavigationException(string? message, HtmlNode? node) : base(message) => Node = node;
}
