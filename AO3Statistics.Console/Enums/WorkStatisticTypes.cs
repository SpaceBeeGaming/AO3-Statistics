using System.Diagnostics;

namespace AO3Statistics.ConsoleApp.Enums;

/// <summary>
/// List of the different types of statistics a work on AO3 has.
/// </summary>
public enum WorkStatisticTypes
{
    WorkId,
    WorkName,
    FandomName,
    WordCount,
    Subscriptions,
    Hits,
    Kudos,
    CommentThreads,
    Bookmarks
}

/// <summary>
/// Contains extension methods to interact with <see cref="WorkStatisticTypes"/>.
/// </summary>
public static class WorkStatisticTypesMethods
{

    /// <summary>
    /// Returns a string representation of the specified value.
    /// </summary>
    /// <param name="type">The <see cref="WorkStatisticTypes"/> to fetch.</param>
    /// <param name="_">A dummy to differentiate from other ToString() methods.</param>
    /// <returns>Returns a string representation of the value specified by <paramref name="type"/>. Formatted as they are referenced in AO3 html.</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="UnreachableException"></exception>
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
            _ => throw new UnreachableException("Undefined enum value."),
        };
    }
}