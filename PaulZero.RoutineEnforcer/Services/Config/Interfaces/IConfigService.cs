using PaulZero.RoutineEnforcer.Models;
using System;

namespace PaulZero.RoutineEnforcer.Services.Config.Interfaces
{
    public interface IConfigService
    {
        event Action<ScheduledEvent> EventCreated;

        event Action<ScheduledEvent> EventRemoved;

        event Action<NoComputerPeriod> NoComputerPeriodCreated;

        event Action<NoComputerPeriod> NoComputerPeriodRemoved;

        AppConfiguration GetAppConfiguration();

        void CreateNewNoComputerPeriod(NoComputerPeriod noComputerPeriod);

        void CreateNewScheduledEvent(ScheduledEvent scheduledEvent);

        void RemoveNoComputerPeriod(NoComputerPeriod noComputerPeriod);

        void RemoveNoComputerPeriodById(string noComputerPeriodId);

        void RemoveScheduledEvent(ScheduledEvent scheduledEvent);

        void RemoveScheduledEventById(string scheduledEventId);

        void UpdateNoComputerPeriod(NoComputerPeriod updatedNoComputerPeriod);

        void UpdateScheduledEvent(ScheduledEvent updatedScheduledEvent);
    }
}
