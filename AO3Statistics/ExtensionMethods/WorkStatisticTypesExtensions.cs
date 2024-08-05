using AO3Statistics.Enums;

namespace AO3Statistics.ExtensionMethods;

/// <summary>
/// Provides extension methods for the <see cref="WorkStatisticTypes"/> enum.
/// </summary>
public static class WorkStatisticTypesExtensions
{
    /// <summary>
    /// Converts the <see cref="WorkStatisticTypes"/> to its string representation.
    /// </summary>
    /// <param name="type">The <see cref="WorkStatisticTypes"/> value to convert.</param>
    /// <param name="_">A placeholder parameter to differentiate from the default ToString() method.</param>
    /// <returns>The string representation of the <see cref="WorkStatisticTypes"/> value. Formatted as they are referenced in AO3 html.</returns>
    /// <exception cref="InvalidOperationException">Thrown on undefined Enum value.</exception>
    /// <exception cref="NotImplementedException"></exception>
    public static string ToString(this WorkStatisticTypes type, bool _)
    {
        return type switch
        {
            WorkStatisticTypes.WorkId => throw new NotImplementedException(),
            WorkStatisticTypes.WorkName => throw new NotImplementedException(),
            WorkStatisticTypes.FandomName => "fandom",
            WorkStatisticTypes.WordCount => "words",
            WorkStatisticTypes.Subscriptions => "subscriptions",
            WorkStatisticTypes.Hits => "hits",
            WorkStatisticTypes.Kudos => "kudos",
            WorkStatisticTypes.CommentThreads => "comments",
            WorkStatisticTypes.Bookmarks => "bookmarks",
            _ => throw new InvalidOperationException("Undefined enum value."),
        };
    }
}
