using PaulZero.RoutineEnforcer.Models;
using System;

namespace PaulZero.RoutineEnforcer.Services.Config.Interfaces
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
