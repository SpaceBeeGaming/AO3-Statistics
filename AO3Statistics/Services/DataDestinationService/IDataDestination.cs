using AO3Statistics.Models;

namespace AO3Statistics.Services.DataDestinationService;
public interface IDataDestination
{
    void SaveData(StatisticsSnapshotModel statisticsSnapshot);
}