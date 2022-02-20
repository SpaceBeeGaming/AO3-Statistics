using System.Runtime.Serialization;

namespace AO3Statistics;

[Serializable]
public class NavigatorException : Exception
{
    public string? Path { get; }
    public bool? IsRecoverable { get; }

    public NavigatorException() : base() { }
    public NavigatorException(string? message) : base(message) { }
    public NavigatorException(string? message, Exception? innerException) : base(message, innerException) { }
    public NavigatorException(string? message, string? path) : this(message)
    {
        Path = path;
    }

    public NavigatorException(string? message, string? path, bool? isRecoverable) : this(message)
    {
        Path = path;
        IsRecoverable = isRecoverable;
    }

    protected NavigatorException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
}
