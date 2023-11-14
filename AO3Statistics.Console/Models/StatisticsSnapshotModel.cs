using System.Text;

namespace AO3Statistics.ConsoleApp.Models;

/// <summary>
/// Contains the combined user and work statistics.
/// </summary>
public sealed class StatisticsSnapshotModel
{
    /// <summary>
    /// Date of creation.
    /// </summary>
    public DateOnly Date { get; init; }
    /// <summary>
    /// A list containing the statistics for users works.
    /// </summary>
    public required List<WorkStatisticsModel> WorkStatistics { get; init; }

    /// <summary>
    /// Contains the users statistics.
    /// </summary>
    public required UserStatisticsModel UserStatistics { get; init; }

    /// <summary>
    /// Returns the combined user and work statistics in a pretty print format.
    /// </summary>
    /// <param name="_">A dummy to differentiate from other ToString() methods.</param>
    /// <returns>A formatted string.</returns>
    public string ToString(bool _)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"Date:               {Date.ToString("o")}");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("User Statistics:");
        stringBuilder.AppendLine(UserStatistics.ToString(true));
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Work Statistics:");
        foreach (WorkStatisticsModel model in WorkStatistics)
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(model.ToString(true));
        }

        return stringBuilder.ToString();
    }
}
