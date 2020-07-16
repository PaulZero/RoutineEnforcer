using PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces;
using System.Threading;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock
{
    internal class ClockServiceCancellationProvider : IClockServiceCancellationProvider
    {
        public bool IsExecuting
        {
            get
            {
                if (_cancellationTokenSource is null)
                {
                    return false;
                }

                return !_cancellationTokenSource.IsCancellationRequested;
            }
        }

        public CancellationToken Token => _cancellationTokenSource?.Token ?? CancellationToken.None;

        private CancellationTokenSource _cancellationTokenSource;

        public void Cancel()
        {
            if (_cancellationTokenSource != null)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }

                _cancellationTokenSource.Dispose();
            }
        }

        public void Prepare()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}
