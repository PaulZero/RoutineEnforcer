using Microsoft.Extensions.Logging;
using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PaulZero.RoutineEnforcer.Services.Config
{
    internal class ConfigService : IConfigService
    {
        public event Action<ScheduledEvent> EventCreated;

        public event Action<ScheduledEvent> EventRemoved;

        public event Action<NoComputerPeriod> NoComputerPeriodCreated;

        public event Action<NoComputerPeriod> NoComputerPeriodRemoved;

        private readonly AppConfiguration _configuration;
        private readonly ILogger _logger;

        public AppConfiguration GetAppConfiguration()
        {
            return _configuration;
        }

        public ConfigService(ILogger<IConfigService> logger)
        {
            _logger = logger;
            _configuration = LoadFromFile();
        }

        public void CreateNewNoComputerPeriod(NoComputerPeriod noComputerPeriod)
        {
            try
            {
                _logger.LogDebug($"Createing no computer period '{noComputerPeriod?.Name}' and saving it to configuration.");

                _configuration.NoComputerPeriods.Add(noComputerPeriod);

                NoComputerPeriodCreated?.Invoke(noComputerPeriod);

                SaveToFile();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to save no computer period to configuration.");
            }
        }

        public void CreateNewScheduledEvent(ScheduledEvent scheduledEvent)
        {
            try
            {
                _logger.LogDebug($"Creating scheduled event '{scheduledEvent?.Name}' and saving it to configuration.");

                _configuration.ScheduledEvents.Add(scheduledEvent);

                EventCreated?.Invoke(scheduledEvent);

                SaveToFile();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to save scheduled event to configuration.");
            }
        }

        public void RemoveNoComputerPeriod(NoComputerPeriod noComputerPeriod)
        {
            try
            {
                _logger.LogDebug($"Removing no computer period '{noComputerPeriod?.Name}' from configuration.");

                _configuration.NoComputerPeriods.Remove(noComputerPeriod);

                NoComputerPeriodRemoved?.Invoke(noComputerPeriod);

                SaveToFile();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to remove no computer period from configuration.");
            }
        }

        public void RemoveScheduledEvent(ScheduledEvent scheduledEvent)
        {
            try
            {
                _logger.LogDebug($"Removing scheduled event '{scheduledEvent?.Name}' from configuration.");

                _configuration.ScheduledEvents.Remove(scheduledEvent);

                EventRemoved?.Invoke(scheduledEvent);

                SaveToFile();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to remove scheduled event from configuration.");
            }
        }

        private AppConfiguration LoadFromFile()
        {
            try
            {
                var filePath = GetConfigFilePath();

                _logger.LogDebug($"Attempting to load configuration from '{filePath}'");

                if (!File.Exists(filePath))
                {
                    _logger.LogDebug("Configuration file does not exist, creating an empty configuration.");

                    return new AppConfiguration();
                }

                var json = File.ReadAllText(filePath);

                var configuration = JsonSerializer.Deserialize<AppConfiguration>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogDebug($"Configuration file loaded successfully, containing {configuration.ScheduledEvents.Count} event(s).");

                return configuration;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to load configuration from file.");

                return new AppConfiguration();
            }
        }

        private void SaveToFile()
        {
            try
            {
                var filePath = GetConfigFilePath();

                _logger.LogDebug($"Saving configuration to '{filePath}'");

                var json = JsonSerializer.Serialize(_configuration ?? new AppConfiguration(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                });

                var directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(filePath, json);

                _logger.LogDebug("Saved configuration.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to save configuration file.");
            }
        }

        private string GetConfigFilePath()
        {
            var appDataDirectory = PathUtilities.GetProgramDataDirectory();

            return Path.Combine(appDataDirectory, "routine-config.json");
        }
    }
}
