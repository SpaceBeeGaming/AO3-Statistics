using System.Diagnostics;

namespace AO3Statistics.ConsoleApp.Enums;

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

public static class UserStatisticTypesMethods
{
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
