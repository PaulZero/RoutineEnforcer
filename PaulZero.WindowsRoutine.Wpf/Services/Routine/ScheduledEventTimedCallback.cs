using PaulZero.WindowsRoutine.Wpf.Models;
using PaulZero.WindowsRoutine.Wpf.Services.Clock;
using System;
using System.Threading.Tasks;

namespace PaulZero.WindowsRoutine.Wpf.Services.Routine
{
    internal class ScheduledEventTimedCallback : TimedCallback
    {
        public ScheduledEvent ScheduledEvent { get; }

        public string ScheduledEventId => ScheduledEvent.Id;

        public ScheduledEventTimedCallback(ScheduledEvent scheduledEvent, Action<ScheduledEvent> eventHandler)
            : base(() => eventHandler(scheduledEvent), scheduledEvent.WarningTime.Hours, scheduledEvent.WarningTime.Minutes)
        {
            ScheduledEvent = scheduledEvent;
        }

        public override bool IsDue(DateTime currentDateTime)
        {
            if (!base.IsDue(currentDateTime))
            {
                return false;
            }

            if (ScheduledEvent.DaysScheduled is null)
            {
                return true;
            }

            return ScheduledEvent.DaysScheduled.IsValidFor(currentDateTime.DayOfWeek);
        }
    }
}
