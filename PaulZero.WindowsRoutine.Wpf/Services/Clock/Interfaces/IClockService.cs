using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces
{
    public interface IClockService : IDisposable
    {
        void RegisterCallback(TimedCallback callback);

        bool RemoveCallback(string callbackId);

        void RemoveAllCallbacks();

        void Start();

        void Stop();
    }
}
