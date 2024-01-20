namespace AO3Statistics.ConsoleApp.Models;

public sealed class UserStatisticsModel
{
    public DateOnly Date { get; set; }
    public required string Username { get; init; }
    public required int UserSubscriptions { get; init; }
    public required int Kudos { get; init; }
    public required int CommentThreads { get; init; }
    public required int Bookmarks { get; init; }
    public required int Subscriptions { get; init; }
    public required int WordCount { get; init; }
    public required int Hits { get; init; }

    public string ToString(bool _)
    {
        return $"""
        User Subscriptions: {UserSubscriptions}
        Kudos:              {Kudos}
        Comment Threads:    {CommentThreads}
        Bookmarks:          {Bookmarks}
        Subscriptions:      {Subscriptions}
        Word Count:         {WordCount}
        Hits:               {Hits}
        """;
    }
}
