using System;

namespace PaulZero.RoutineEnforcer.Services.Clock.Interfaces
{
    public interface IClockService : IDisposable
    {
        void RegisterCallback(ITimedCallback callback);

        bool RemoveCallback(string callbackId);

        void RemoveAllCallbacks();

        void Start();

        void Stop();
    }
}
