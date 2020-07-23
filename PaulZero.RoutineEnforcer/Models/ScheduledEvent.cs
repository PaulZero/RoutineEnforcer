using PaulZero.RoutineEnforcer.Models.Serialisation;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace PaulZero.RoutineEnforcer.Models
{
    public class ScheduledEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan WarningTime { get; set; }

        [JsonConverter(typeof(DaySelectionConverter))]
        public DaySelection DaysScheduled { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan ActionDelay { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventActionType ActionType { get; set; } = EventActionType.LockScreen;

        public DateTime GetNextDueDate()
        {
            var todayDateTime = DateTime.Today.Add(WarningTime);

            if (DateTime.Now < todayDateTime)
            {
                return todayDateTime;
            }

            var currentDate = todayDateTime.AddDays(1);

            if (!DaysScheduled.GetEnabledDays().Any())
            {
                return currentDate;
            }

            while (true)
            {
                if (DaysScheduled.IsValidFor(currentDate.DayOfWeek))
                {
                    return currentDate;
                }

                currentDate = currentDate.AddDays(1);
            }

            throw new Exception("Unable to find next due date for scheduled task!");
        }
    }
}
