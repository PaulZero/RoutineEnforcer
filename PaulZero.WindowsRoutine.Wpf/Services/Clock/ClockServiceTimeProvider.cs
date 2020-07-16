using PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces;
using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock
{
    internal class ClockServiceTimeProvider : IClockServiceTimeProvider
    {
        public TimeSpan GetClockInterval()
            => TimeSpan.FromMinutes(1);

        public DateTime GetCurrentTime()
            => DateTime.Now;
    }
}
