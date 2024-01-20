using System.Diagnostics;

namespace AO3Statistics.ConsoleApp.Enums;

/// <summary>
/// List of the different types of statistics a user on AO3 has.
/// </summary>
public enum UserStatisticTypes
{
    UserSubscriptions,
    Kudos,
    CommentThreads,
    Bookmarks,
    Subscriptions,
    WordCount,
    Hits,
}

/// <summary>
/// Contains extension methods to interact with <see cref="UserStatisticTypes"/>.
/// </summary>
public static class UserStatisticTypesMethods
{

    /// <summary>
    /// Returns a string representation of the specified value.
    /// </summary>
    /// <param name="type">The <see cref="UserStatisticTypes"/> to fetch.</param>
    /// <param name="_">A dummy to differentiate from other ToString() methods.</param>
    /// <returns>Returns a string representation of the value specified by <paramref name="type"/>. Formatted as they are referenced in AO3 html.</returns>
    /// <exception cref="UnreachableException"></exception>
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
            _ => throw new UnreachableException("Undefined Enum Value."),
        };
    }
}
