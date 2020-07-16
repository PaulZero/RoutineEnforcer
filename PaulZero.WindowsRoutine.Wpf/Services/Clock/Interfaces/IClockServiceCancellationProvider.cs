using System;
using System.Threading;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces
{
    public interface IClockServiceCancellationProvider : IDisposable
    {
        bool IsExecuting { get; }

        CancellationToken Token { get; }

        void Cancel();

        void Prepare();
    }
}
