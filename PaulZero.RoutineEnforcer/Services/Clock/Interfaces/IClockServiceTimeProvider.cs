using System;

namespace PaulZero.RoutineEnforcer.Services.Clock.Interfaces
{
    public interface IClockServiceTimeProvider
    {
        TimeSpan GetClockInterval();

        DateTime GetCurrentTime();
    }
}
