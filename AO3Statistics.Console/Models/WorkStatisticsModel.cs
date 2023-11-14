namespace AO3Statistics.ConsoleApp.Models;
public sealed class WorkStatisticsModel
{
    public required string WorkId { get; init; }
    public required string WorkName { get; init; }
    public required string FandomName { get; init; }
    public required int WordCount { get; init; }
    public required int Subscriptions { get; init; }
    public required int Hits { get; init; }
    public required int Kudos { get; init; }
    public required int CommentThreads { get; init; }
    public required int Bookmarks { get; init; }

    public string ToString(bool _)
    {
        return $"""
        Work name:          {WorkName}
        Fandom:             {FandomName}
        Word count:         {WordCount}
        Subscriptions:      {Subscriptions}
        Hits:               {Hits}
        Kudos:              {Kudos}
        Comment Threads:    {CommentThreads}
        Bookmarks:          {Bookmarks}
        """;
    }
}
