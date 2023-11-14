using System.Diagnostics;

namespace AO3Statistics.ConsoleApp.Enums;

public enum WorkStatisticTypes
{
    WorkName,
    FandomName,
    WordCount,
    Subscriptions,
    Hits,
    Kudos,
    CommentThreads,
    Bookmarks
}

public static class WorkStatisticTypesMethods
{
    public static string ToString(this WorkStatisticTypes type, bool _)
    {
        return type switch
        {
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