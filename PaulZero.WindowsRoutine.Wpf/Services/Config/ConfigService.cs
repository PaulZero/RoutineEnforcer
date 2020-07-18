using Microsoft.Extensions.Logging;
using PaulZero.WindowsRoutine.Wpf.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PaulZero.WindowsRoutine.Wpf.Services.Config
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

                // JON: I've set the first event here to happen immediately, but take an hour so there's minimal chance of inconvenience, you'll need to have at least one configured though.

                var firstEvent = configuration.ScheduledEvents.FirstOrDefault(s => s.Name == "Do some debugging");

                if (firstEvent != null)
                {
                    firstEvent.WarningTimeHour = DateTime.Now.Hour;
                    firstEvent.WarningTimeMinute = DateTime.Now.Minute;
                    firstEvent.ActionDelayMinutes = 60;
                    firstEvent.ActionType = EventActionType.LockScreen;
                    firstEvent.DaysScheduled = DaySelection.Daily;
                }
                else
                {
                    configuration.ScheduledEvents.Add(new ScheduledEvent
                    {
                        Name = "Do some debugging",
                        WarningTimeHour = DateTime.Now.Hour,
                        WarningTimeMinute = DateTime.Now.Minute,
                        ActionDelayMinutes = 60,
                        ActionType = EventActionType.LockScreen,
                        DaysScheduled = DaySelection.Daily
                    });
                }

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
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            return Path.Combine(programData, "PaulZero", "WindowsRoutine", "config.json");
        }
    }
}
