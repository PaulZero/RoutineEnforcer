using PaulZero.RoutineEnforcer.Models;
using System;
using System.Linq;

namespace PaulZero.RoutineEnforcer.Views.Models
{
    public class NoComputerPeriodViewModel
    {
        public string Id => _noComputerPeriod.Id;

        public string Name => _noComputerPeriod.Name;

        public DateTime NextDueDate => _noComputerPeriod.GetNextDueDate(DateTime.Now);

        public string ActionSummary => CreateActionSummary();

        public string SleepSummary => CreateSleepSummary();

        public string NextDueSummary => CreateNextDueSummary();

        private readonly NoComputerPeriod _noComputerPeriod;

        public NoComputerPeriodViewModel(NoComputerPeriod noComputerPeriod)
        {
            _noComputerPeriod = noComputerPeriod;
        }

        private string CreateActionSummary()
        {
            var endTime = _noComputerPeriod.EndTime;
            var formattedEndTime = $"{endTime.Hours:00}:{endTime.Minutes:00}";
            var delayMinutes = _noComputerPeriod.ActionDelay.Minutes;

            return $"The computer will be put to sleep. If you start the computer up again before {formattedEndTime} you will have {delayMinutes} before it goes back to sleep.";
        }

        private string CreateSleepSummary()
        {
            var startTime = _noComputerPeriod.StartTime;
            var formattedStartTime = $"{startTime.Hours:00}:{startTime.Minutes:00}";
            var delayMinutes = _noComputerPeriod.ActionDelay.Minutes;


            return $"Starts at {formattedStartTime} {CreateDailyFrequencyText()}, and expires after {delayMinutes} minutes.";
        }

        private string CreateDailyFrequencyText()
        {
            var enabledDays = _noComputerPeriod.DaysActive.GetEnabledDays();

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
            if (_noComputerPeriod.IsActiveAt(DateTime.Now))
            {
                return "Currently active.";
            }

            var timeRemaining = NextDueDate - DateTime.Now;

            if (NextDueDate.Date == DateTime.Today)
            {
                if (timeRemaining.Hours >= 1)
                {
                    return $"Next due in {timeRemaining.Hours} hour{(timeRemaining.Hours > 1 ? "s" : "")}";
                }

                if (timeRemaining.Minutes >= 1)
                {
                    return $"Next due in {timeRemaining.Minutes} minute{(timeRemaining.Minutes > 1 ? "s" : "")}";
                }

                return $"Next due in {timeRemaining.Seconds} second{(timeRemaining.Seconds > 1 ? "s" : "")}";
            }

            if (timeRemaining.Days <= 1)
            {
                return "Next due tomorrow";
            }

            return $"Next due in {timeRemaining.Days} days";
        }
    }
}
