using System;
using System.Linq;

namespace PaulZero.WindowsRoutine.Wpf.Models.View
{
    public class ScheduledEventViewModel
    {
        public string Name => _scheduledEvent.Name;

        public DateTime NextDueDate => _scheduledEvent.GetNextDueDate();

        public DaySelection DaySelection => _scheduledEvent.DaysScheduled;

        public TimeSpan WarningTime => _scheduledEvent.WarningTime;

        public TimeSpan ActionDelay => _scheduledEvent.ActionDelay;

        public EventActionType ActionType => _scheduledEvent.ActionType;

        public string NextDueSummary => CreateNextDueSummary();

        public string DailyFrequencyText => CreateDailyFrequencyText();

        public string WarningTimeText => $"{WarningTime.Hours:00}:{WarningTime.Minutes:00} {DailyFrequencyText}";

        public string ActionDelayDext => ActionDelay.Minutes == 1 ? "1 minute" : $"{ActionDelay.Minutes} minutes";

        public string ActionDescriptionText => ActionType == EventActionType.LockScreen ? "Screen will be locked" : "PC will be put to sleep";

        public string EventDescription => $"Starts at {WarningTimeText}, and expires after {ActionDelayDext}.";

        private readonly ScheduledEvent _scheduledEvent;

        public ScheduledEventViewModel(ScheduledEvent scheduledEvent)
        {
            _scheduledEvent = scheduledEvent;
        }

        private string CreateDailyFrequencyText()
        {
            var enabledDays = DaySelection.GetEnabledDays();

            if (!enabledDays.Any() || enabledDays.Length == 7)
            {
                return "daily";
            }

            if (enabledDays.Length == 1)
            {
                return $"every {enabledDays[0]}";
            }

            if (enabledDays.Length == 2)
            {
                return $"every {enabledDays[0]} and {enabledDays[1]}";
            }

            var startingDays = enabledDays.Take(enabledDays.Length - 1);
            var lastDay = enabledDays.Last();

            return $"every {string.Join(", ", startingDays)} and {lastDay}";
        }

        private string CreateNextDueSummary()
        {
            var timeRemaining = NextDueDate - DateTime.Now;

            if (NextDueDate.Date == DateTime.Today)
            {
                if (timeRemaining.Hours >= 1)
                {
                    return $"in {timeRemaining.Hours} hour{(timeRemaining.Hours > 1 ? "s" : "")}";
                }

                if (timeRemaining.Minutes >= 1)
                {
                    return $"in {timeRemaining.Minutes} minute{(timeRemaining.Minutes > 1 ? "s" : "")}";
                }

                return $"in {timeRemaining.Seconds} second{(timeRemaining.Seconds > 1 ? "s" : "")}";
            }

            if (timeRemaining.Days <= 1)
            {
                return "tomorrow";
            }

            return $"in {timeRemaining.Days} days";
        }
    }
}
