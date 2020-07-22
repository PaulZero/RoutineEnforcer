using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using System;

namespace PaulZero.RoutineEnforcer.Services.Clock
{
    internal class ClockServiceTimeProvider : IClockServiceTimeProvider
    {
        public TimeSpan GetClockInterval()
            => TimeSpan.FromMinutes(1);

        public DateTime GetCurrentTime()
            => DateTime.Now;
    }
}
