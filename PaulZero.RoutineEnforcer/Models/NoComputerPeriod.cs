using PaulZero.RoutineEnforcer.Models.Serialisation;
using System;
using System.Text.Json.Serialization;

namespace PaulZero.RoutineEnforcer.Models
{
    public class NoComputerPeriod
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        [JsonConverter(typeof(DaySelectionConverter))]
        public DaySelection DaysActive { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan StartTime { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan EndTime { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan ActionDelay { get; set; }

        public bool IsActiveAt(DateTime dateTime)
        {
            var isActiveToday = DaysActive.IsValidFor(dateTime.DayOfWeek);
            var wasActiveYesterday = DaysActive.IsValidFor(dateTime.AddDays(-1).DayOfWeek);

            var currentTime = dateTime.TimeOfDay;

            if (EndTime < StartTime)
            {
                if (currentTime < EndTime && wasActiveYesterday)
                {
                    return true;
                }

                if (currentTime >= StartTime && isActiveToday)
                {
                    return true;
                }

                return false;
            }

            // this period is contained entirely within one day
            return isActiveToday && currentTime >= StartTime && currentTime < EndTime;
        }

        public DateTime GetNextDueDate()
        {
            return DateTime.Now;
        }
    }
}
