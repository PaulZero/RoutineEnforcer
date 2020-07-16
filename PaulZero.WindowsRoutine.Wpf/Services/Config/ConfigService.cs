using PaulZero.WindowsRoutine.Wpf.Models;
using System;
using System.IO;
using System.Text.Json;

namespace PaulZero.WindowsRoutine.Wpf.Services.Config
{
    internal class ConfigService : IConfigService
    {
        public event Action<ScheduledEvent> EventCreated;

        public event Action<ScheduledEvent> EventRemoved;

        private readonly AppConfiguration _configuration;

        public AppConfiguration GetAppConfiguration()
        {
            return _configuration;
        }

        public ConfigService()
        {
            _configuration = LoadFromFile();
        }

        public void CreateNewScheduledEvent(ScheduledEvent scheduledEvent)
        {
            _configuration.ScheduledEvents.Add(scheduledEvent);

            EventCreated?.Invoke(scheduledEvent);

            SaveToFile();
        }

        public void RemoveScheduledEvent(ScheduledEvent scheduledEvent)
        {
            _configuration.ScheduledEvents.Remove(scheduledEvent);

            EventRemoved?.Invoke(scheduledEvent);

            SaveToFile();
        }

        private AppConfiguration LoadFromFile()
        {
            var filePath = GetConfigFilePath();

            if (!File.Exists(filePath))
            {
                return new AppConfiguration();
            }

            var json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<AppConfiguration>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_configuration ?? new AppConfiguration(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                });

                var filePath = GetConfigFilePath();
                var directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(filePath, json);
            }
            catch
            {
                // TODO: Handle this
            }
        }

        private string GetConfigFilePath()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            
            return Path.Combine(programData, "PaulZero", "WindowsRoutine", "config.json");
        }
    }
}
