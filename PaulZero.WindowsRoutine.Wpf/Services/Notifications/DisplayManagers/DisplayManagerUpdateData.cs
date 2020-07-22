using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications.DisplayManagers
{
    public class DisplayManagerUpdateData
    {
        public uint UpdateNumber { get; }

        public int PercentageProgress { get; }

        public string FractionProgress { get; }

        public string RemainingTime { get; }


        public DisplayManagerUpdateData(uint updateNumber, TimeSpan delay, TimeSpan remaining)
        {
            UpdateNumber = updateNumber;

            var percentageProgress = Math.Round(100 / delay.TotalSeconds * remaining.TotalSeconds);

            FractionProgress = string.Format("{0:N2}", percentageProgress / 100);
            PercentageProgress = (int)percentageProgress;
            RemainingTime = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
    }
}
