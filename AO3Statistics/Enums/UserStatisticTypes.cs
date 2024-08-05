namespace AO3Statistics.Enums;

/// <summary>
/// List of the different types of statistics a user on AO3 has.
/// </summary>
public enum UserStatisticTypes
{
    /// <summary>
    /// Represents the number of user subscriptions.
    /// </summary>
    UserSubscriptions,

    /// <summary>
    /// Represents the number of kudos received.
    /// </summary>
    Kudos,

    /// <summary>
    /// Represents the number of comment threads.
    /// </summary>
    CommentThreads,

    /// <summary>
    /// Represents the number of bookmarks.
    /// </summary>
    Bookmarks,

    /// <summary>
    /// Represents the number of subscriptions.
    /// </summary>
    Subscriptions,

    /// <summary>
    /// Represents the word count.
    /// </summary>
    WordCount,

    /// <summary>
    /// Represents the number of hits.
    /// </summary>
    Hits,
}

/// <summary>
/// Contains extension methods to interact with <see cref="UserStatisticTypes"/>.
/// </summary>
public static class UserStatisticTypesMethods
{
    /// <summary>
    /// Converts the <see cref="UserStatisticTypes"/> value to its string representation.
    /// </summary>
    /// <param name="type">The <see cref="UserStatisticTypes"/> value to convert.</param>
    /// <param name="_">A placeholder parameter to differentiate from the default ToString method.</param>
    /// <returns>The string representation of the <see cref="UserStatisticTypes"/> value. Formatted as they are referenced in AO3 html.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an undefined enum value is encountered.</exception>
    public static string ToString(this UserStatisticTypes type, bool _)
    {
        return type switch
        {
            UserStatisticTypes.UserSubscriptions => "user subscriptions",
            UserStatisticTypes.Kudos => "kudos",
            UserStatisticTypes.CommentThreads => "comment thread count",
            UserStatisticTypes.Bookmarks => "bookmarks",
            UserStatisticTypes.Subscriptions => "subscriptions",
            UserStatisticTypes.WordCount => "words",
            UserStatisticTypes.Hits => "hits",
            _ => throw new InvalidOperationException("Undefined Enum Value."),
        };
    }
}
