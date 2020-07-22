using System;

namespace PaulZero.RoutineEnforcer.Services.Notifications.NotificationViews
{
    public struct NotificationUpdateData
    {
        public uint UpdateNumber { get; }

        public int PercentageProgress { get; }

        public string FractionProgress { get; }

        public string RemainingTime { get; }


        public NotificationUpdateData(uint updateNumber, TimeSpan delay, TimeSpan remaining)
        {
            UpdateNumber = updateNumber;

            var percentageProgress = Math.Round(100 / delay.TotalSeconds * remaining.TotalSeconds);

            FractionProgress = string.Format("{0:N2}", percentageProgress / 100);
            PercentageProgress = (int)percentageProgress;
            RemainingTime = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
    }
}
