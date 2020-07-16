using System;
using System.Collections.Generic;
using System.Threading;

namespace PaulZero.WindowsRoutine.Wpf.Services.Timing
{
    internal class TimingService : ITimingService
    {
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly List<TimedTask> _scheduledTasks = new List<TimedTask>();

        public void RegisterCallback(TimeSpan scheduledTime, Action callback)
        {
            var task = new TimedTask(scheduledTime, callback, _cancellationTokenSource.Token, _syncContext);

            task.Run();

            _scheduledTasks.Add(task);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            

            foreach (var task in _scheduledTasks)
            {
                task.Dispose();
            }

            _cancellationTokenSource.Dispose();

            _scheduledTasks.Clear();
        }
    }
}
