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
#if DEBUG
                _logger.LogInformation("Creating debug configuration.");

                return new AppConfiguration
                {
                    //ScheduledEvents = new List<ScheduledEvent>
                    //{
                    //    new ScheduledEvent
                    //    {
                    //        WarningTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0),
                    //        ActionDelay = TimeSpan.FromMinutes(60),
                    //        ActionType = EventActionType.LockScreen,
                    //        DaysScheduled = DaySelection.Daily,
                    //        Id = Guid.NewGuid().ToString(),
                    //        Name = "Do some debugging!"
                    //    }
                    //}
                    NoComputerPeriods = new List<NoComputerPeriod>
                    {
                        new NoComputerPeriod
                        {
                            Name = "You should not be on your computer, idiot.",
                            StartTime = DateTime.Now.AddHours(-1).TimeOfDay,
                            EndTime = DateTime.Now.AddHours(1).TimeOfDay,
                            ActionDelay = TimeSpan.FromSeconds(30),
                            DaysActive = DaySelection.Daily
                        }
                    }
                };
#else
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
#endif
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
