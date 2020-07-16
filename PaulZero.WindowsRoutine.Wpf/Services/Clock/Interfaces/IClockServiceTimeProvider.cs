using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces
{
    public interface IClockServiceTimeProvider
    {
        TimeSpan GetClockInterval();

        DateTime GetCurrentTime();
    }
}
