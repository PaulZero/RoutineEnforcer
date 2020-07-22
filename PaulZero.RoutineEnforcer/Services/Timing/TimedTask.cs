using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaulZero.WindowsRoutine.Wpf.Services.Timing
{
    internal class TimedTask : IDisposable
    {
        public DateTime ScheduledTime { get; }

        public TimeSpan TimeRemaining => ScheduledTime - DateTime.Now;

        private readonly TimeSpan _scheduledTime;
        private readonly Action _callback;
        private readonly CancellationToken _cancellationToken;
        private readonly SynchronizationContext _syncContext;
        private Task _task;

        public TimedTask(TimeSpan scheduledTime, Action callback, CancellationToken cancellationToken, SynchronizationContext syncContext)
        {
            _scheduledTime = scheduledTime;
            _callback = callback;
            _cancellationToken = cancellationToken;
            _syncContext = syncContext;

            ScheduledTime = CalculateScheduledTime();
        }

        public void Run()
        {
            var delay = ScheduledTime - DateTime.Now;

            _task = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delay, _cancellationToken);

                    _syncContext.Post(s => _callback(), null);
                }
                catch
                {
                    // TODO: Handle this!
                }
            });
        }

        private DateTime CalculateScheduledTime()
            => DateTime.Today.Add(_scheduledTime);

        public void Dispose()
        {
            _task?.Dispose();
        }
    }
}
