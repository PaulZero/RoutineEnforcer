using PaulZero.WindowsRoutine.Wpf.Models;
using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Config
{
    internal interface IConfigService
    {
        event Action<ScheduledEvent> EventCreated;

        event Action<ScheduledEvent> EventRemoved;

        AppConfiguration GetAppConfiguration();

        void CreateNewScheduledEvent(ScheduledEvent scheduledEvent);

        void RemoveScheduledEvent(ScheduledEvent scheduledEvent);
    }
}
