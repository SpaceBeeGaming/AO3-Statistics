using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace AO3Statistics.Models;

/// <summary>
/// Contains the combined user and work statistics.
/// </summary>
public sealed class StatisticsSnapshotModel(DateOnly date)
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
        get => _workStatistics;

        [MemberNotNull(nameof(_workStatistics))]
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _workStatistics = value;
            _workStatistics.ForEach(x => x.Date = Date);
        }
    }

    /// <summary>
    /// Contains the users statistics.
    /// </summary>
    public required UserStatisticsModel UserStatistics
    {
        get => _userStatistics;
        [MemberNotNull(nameof(_userStatistics))]
        init
        {
            ArgumentNullException.ThrowIfNull(value);
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
        stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"Date:               {Date:o}");
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
