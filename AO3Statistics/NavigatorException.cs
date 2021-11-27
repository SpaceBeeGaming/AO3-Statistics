using System.Runtime.Serialization;

namespace AO3Statistics;

[Serializable]
public class NavigatorException : Exception
{
    public string? Path { get; }
    public NavigatorException() : base() { }
    public NavigatorException(string? message) : base(message) { }
    public NavigatorException(string? message, Exception? innerException) : base(message, innerException) { }
    public NavigatorException(string? message, string? path) : this(message)
    {
        Path = path;
    }

    protected NavigatorException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
}
