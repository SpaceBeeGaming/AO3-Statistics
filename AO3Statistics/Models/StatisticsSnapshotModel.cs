using System.Text;

namespace AO3Statistics.Models;

/// <summary>
/// Contains the combined user and work statistics.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public sealed class StatisticsSnapshotModel(DateOnly date)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
{
    private List<WorkStatisticsModel> _workStatistics;
    private UserStatisticsModel _userStatistics;

    /// <summary>
    /// Date of creation.
    /// </summary>
    public DateOnly Date { get; init; } = date;
    /// <summary>
    /// A list containing the statistics for users works.
    /// </summary>
    public required List<WorkStatisticsModel> WorkStatistics
    {
        get => _workStatistics; init
        {
            _workStatistics = value;
            _workStatistics.ForEach(x => x.Date = Date);
        }
    }

    /// <summary>
    /// Contains the users statistics.
    /// </summary>
    public required UserStatisticsModel UserStatistics
    {
        get => _userStatistics; init
        {
            _userStatistics = value;
            _userStatistics.Date = Date;
        }
    }

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
