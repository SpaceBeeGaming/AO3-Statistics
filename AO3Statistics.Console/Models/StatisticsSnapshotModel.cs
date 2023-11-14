using System.Text;

namespace AO3Statistics.ConsoleApp.Models;
public sealed class StatisticsSnapshotModel
{
    public DateOnly Date { get; set; }
    public required List<WorkStatisticsModel> WorkStatistics { get; init; }
    public required UserStatisticsModel UserStatistics { get; init; }

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
