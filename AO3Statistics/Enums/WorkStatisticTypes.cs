namespace AO3Statistics.Enums;

/// <summary>
/// List of the different types of statistics a work on AO3 has.
/// </summary>
public enum WorkStatisticTypes
{
    /// <summary>
    /// Represents the ID of the work.
    /// </summary>
    WorkId,

    /// <summary>
    /// Represents the name of the work.
    /// </summary>
    WorkName,

    /// <summary>
    /// Represents the name of the fandom.
    /// </summary>
    FandomName,

    /// <summary>
    /// Represents the word count of the work.
    /// </summary>
    WordCount,

    /// <summary>
    /// Represents the number of subscriptions for the work.
    /// </summary>
    Subscriptions,

    /// <summary>
    /// Represents the number of hits/views for the work.
    /// </summary>
    Hits,

    /// <summary>
    /// Represents the number of kudos for the work.
    /// </summary>
    Kudos,

    /// <summary>
    /// Represents the number of comment threads for the work.
    /// </summary>
    CommentThreads,

    /// <summary>
    /// Represents the number of bookmarks for the work.
    /// </summary>
    Bookmarks
}

/// <summary>
/// Contains extension methods to interact with <see cref="WorkStatisticTypes"/>.
/// </summary>
public static class WorkStatisticTypesMethods
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
