namespace AO3Statistics;

/// <summary>
/// Exception that is thrown on <see cref="StatModel"/> deserialization error.
/// </summary>
public sealed class StatModelException : Exception
{
    /// <summary>
    /// Contains the elements that were involved with this exception.
    /// </summary>
    public string[]? Elements { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatModelException"/> class.
    /// </summary>
    public StatModelException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatModelException"/> class with a specified error <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public StatModelException(string? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatModelException"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The <see cref="Exception"/> that is the cause of the current exception, 
    /// or a <see langword="null"/> reference if no inner exception is specified.</param>
    public StatModelException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatModelException"/> class with a specified error <paramref name="message"/> and the <paramref name="elements"/> which caused this exception.
    /// </summary>
    /// <param name="elements">The elements that resulted in this exception.</param>
    /// <param name="message">The message that describes the error.</param>
    public StatModelException(string[]? elements, string? message) : this(message)
    {
        Elements = elements;
    }
}