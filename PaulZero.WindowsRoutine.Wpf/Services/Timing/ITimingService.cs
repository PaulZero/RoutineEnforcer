using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Timing
{
    internal interface ITimingService : IDisposable
    {
        void RegisterCallback(TimeSpan scheduledTime, Action callback);

        
    }
}
