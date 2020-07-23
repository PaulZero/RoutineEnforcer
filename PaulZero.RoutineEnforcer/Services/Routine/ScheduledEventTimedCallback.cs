using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Services.Clock;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using System;

namespace PaulZero.RoutineEnforcer.Services.Routine
{
    internal class ScheduledEventTimedCallback : AbstractTimedCallback
    {
        public ScheduledEvent ScheduledEvent { get; }

        public string ScheduledEventId => ScheduledEvent.Id;

        public override bool IsPeriod => false;

        public ScheduledEventTimedCallback(ScheduledEvent scheduledEvent, Action<ITimedCallback> eventHandler)
            : base(eventHandler)
        {
            ScheduledEvent = scheduledEvent;
        }

        public override bool IsDue(DateTime currentDateTime)
        {
            if (ScheduledEvent.DaysScheduled.IsValidFor(currentDateTime.DayOfWeek))
            {
                return IsDueAtCurrentTime(currentDateTime);
            }

            return false;
        }

        private bool IsDueAtCurrentTime(DateTime currentDateTime)
        {
            return
                currentDateTime.Hour == ScheduledEvent.WarningTime.Hours &&
                currentDateTime.Minute == ScheduledEvent.WarningTime.Minutes;
        }
    }
}
