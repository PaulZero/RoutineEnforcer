using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace PaulZero.WindowsRoutine.Wpf.Models
{
    public class ScheduledEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        /// <summary>
        /// 24 hour time representation of the time the first warning will be displayed.
        /// </summary>
        [JsonIgnore]
        public TimeSpan WarningTime { get; set; }

        public int WarningTimeHour
        {
            get => WarningTime.Hours;
            set
            {
                WarningTime = new TimeSpan(value, WarningTime.Minutes, 0);
            }
        }

        public int WarningTimeMinute
        {
            get => WarningTime.Minutes;
            set
            {
                WarningTime = new TimeSpan(WarningTime.Hours, value, 0);
            }
        }

        public DaySelection DaysScheduled { get; set; }


        [JsonIgnore]
        public DateTime WarningDateTime => DateTime.Today.Add(WarningTime);

        /// <summary>
        /// Time duration representation of how long after the warning notification action will be taken.
        /// </summary>
        [JsonIgnore]
        public TimeSpan ActionDelay { get; set; }

        public int ActionDelayMinutes
        {
            get => ActionDelay.Minutes;
            set
            {
                if (value < 1)
                {
                    value = 1;
                }

                if (value > 60)
                {
                    value = 60;
                }

                ActionDelay = TimeSpan.FromMinutes(value);
            }
        }

        [JsonIgnore]
        public DateTime ActionDateTime => WarningDateTime.Add(ActionDelay);

        public EventActionType ActionType { get; set; } = EventActionType.LockScreen;

        public DateTime GetNextDueDate()
        {
            if (DateTime.Now < WarningDateTime)
            {
                return WarningDateTime;
            }

            var currentDate = WarningDateTime.AddDays(1);

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
