using System.Runtime.Serialization;

namespace AO3Statistics;

/// <summary>
/// Exception type used by <see cref="Navigator"/>. This class cannot be inherited.
/// </summary>
public sealed class NavigatorException : Exception
{
    /// <summary>
    /// XPath string which caused this exception.
    /// </summary>
    public string? XPath { get; }

    /// <summary>
    /// Indicates if the exception is non-critical.
    /// </summary>
    public bool? IsRecoverable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigatorException"/> class.
    /// </summary>
    public NavigatorException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigatorException"/> class with a specified error <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NavigatorException(string? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigatorException"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The <see cref="Exception"/> that is the cause of the current exception, 
    /// or a <see langword="null"/> reference if no inner exception is specified.</param>
    public NavigatorException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigatorException"/> class with a specified error <paramref name="message"/> and the XPath string which caused this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="xPath">The XPath string which caused this exception.</param>
    public NavigatorException(string? message, string? xPath) : this(message)
    {
        XPath = xPath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigatorException"/> class with a specified error <paramref name="message"/> and the XPath string which caused this exception. Adiitionally specifies if the exception is critical or not.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="xPath">The XPath string which caused this exception.</param>
    /// <param name="isRecoverable">Flag to indicate if the exception should be treated as non-critical.</param>
    public NavigatorException(string? message, string? xPath, bool? isRecoverable) : this(message)
    {
        XPath = xPath;
        IsRecoverable = isRecoverable;
    }
}
