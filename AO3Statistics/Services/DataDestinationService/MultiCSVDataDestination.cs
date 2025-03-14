﻿using AO3Statistics.Models;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AO3Statistics.Services.DataDestinationService;
public class MultiCSVDataDestination(IOptions<UserOptions> userOptions, IOptions<OutputOptions> outputOptions, ILogger<MultiCSVDataDestination> logger) : IDataDestination
{
    private readonly IOptions<UserOptions> userOptions = userOptions;
    private readonly IOptions<OutputOptions> outputOptions = outputOptions;
    private readonly ILogger<MultiCSVDataDestination> logger = logger;
    private readonly CsvConfiguration appendConfiguration = new(outputOptions.Value.OutputCulture) { HasHeaderRecord = false };
    private readonly CsvConfiguration newConfiguration = new(outputOptions.Value.OutputCulture);

    public void SaveData(StatisticsSnapshotModel statisticsSnapshot)
    {
        SaveWorkStatistics(statisticsSnapshot.WorkStatistics);
        SaveUserStatistics(statisticsSnapshot.UserStatistics);
    }

    private void SaveWorkStatistics(List<WorkStatisticsModel> workStatistics)
    {
        foreach (WorkStatisticsModel workStatisticsModel in workStatistics)
        {
            try
            {
                string fandom = workStatisticsModel.FandomName.Split(',', StringSplitOptions.TrimEntries).First().Replace(':', '_');
                string fileName = $"{fandom} - {workStatisticsModel.WorkName}.csv";
                string cleanFileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Join(outputOptions.Value.FolderPath, cleanFileName);
                if (File.Exists(filePath))
                {
                    using StreamWriter streamWriter = new(filePath, true);
                    using (CsvWriter csvWriter = new(streamWriter, appendConfiguration))
                    {
                        csvWriter.WriteRecord(workStatisticsModel);
                        csvWriter.NextRecord();
                    }
                }
                else
                {
                    using StreamWriter streamWriter = new(filePath);
                    using (CsvWriter csvWriter = new(streamWriter, newConfiguration))
                    {
                        csvWriter.WriteHeader<WorkStatisticsModel>();
                        csvWriter.NextRecord();
                        csvWriter.WriteRecord(workStatisticsModel);
                        csvWriter.NextRecord();
                    }

                    logger.LogInformation("Created a new file for {WorkName}", workStatisticsModel.WorkName);
                }

                logger.LogInformation("""Work "{WorkName}" saved to "{FilePath}".""", workStatisticsModel.WorkName, Path.GetFullPath(filePath));
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError("Unauthorized access. Skipping.");
                logger.LogDebug(ex, "");
            }
            catch (IOException ex)
            {
                logger.LogError("IO Exception while writing to file. Skipping.");
                logger.LogDebug(ex, "");
            }
        }
    }

    private void SaveUserStatistics(UserStatisticsModel statisticsSnapshot)
    {
        string fileName = $"{userOptions.Value.Username}.csv";
        string cleanFileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
        string filePath = Path.Join(outputOptions.Value.FolderPath, cleanFileName);
        try
        {
            if (File.Exists(filePath))
            {
                using StreamWriter streamWriter = new(filePath, true);
                using (CsvWriter csvWriter = new(streamWriter, appendConfiguration))
                {
                    csvWriter.WriteRecord(statisticsSnapshot);
                    csvWriter.NextRecord();
                }
            }
            else
            {
                using StreamWriter streamWriter = new(filePath);
                using (CsvWriter csvWriter = new(streamWriter, newConfiguration))
                {
                    csvWriter.WriteHeader<UserStatisticsModel>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(statisticsSnapshot);
                    csvWriter.NextRecord();
                }
            }

            logger.LogInformation("""User statistics saved to "{FilePath}".""", Path.GetFullPath(filePath));
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError("Unauthorized access. Skipping.");
            logger.LogDebug(ex, "");
        }
        catch (IOException ex)
        {
            logger.LogError("IO Exception while writing to file. Skipping.");
            logger.LogDebug(ex, "");
        }
    }
}
