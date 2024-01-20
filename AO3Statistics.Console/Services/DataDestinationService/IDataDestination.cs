using AO3Statistics.ConsoleApp.Models;

namespace AO3Statistics.ConsoleApp.Services.DataDestinationService;
public interface IDataDestination
{
    void SaveData(StatisticsSnapshotModel statisticsSnapshot);
}