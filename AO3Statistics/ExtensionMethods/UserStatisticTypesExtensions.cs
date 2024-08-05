using AO3Statistics.Enums;

namespace AO3Statistics.ExtensionMethods;

/// <summary>
/// Provides extension methods for the <see cref="UserStatisticTypes"/> enum.
/// </summary>
public static class UserStatisticTypesExtensions
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
