using PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock
{
    public class ClockService : IClockService
    {
        private Task _timingTask;

        private readonly List<TimedCallback> _callbacks = new List<TimedCallback>();
        private readonly IClockServiceCancellationProvider _cancellationProvider;
        private readonly IClockServiceTimeProvider _timeProvider;
        private DateTime _lastExecutionTime = default;

        public ClockService() : this(new ClockServiceCancellationProvider(), new ClockServiceTimeProvider())
        {
        }

        public ClockService(IClockServiceCancellationProvider cancellationProvider, IClockServiceTimeProvider timeProvider)
        {
            _cancellationProvider = cancellationProvider ?? throw new ArgumentNullException(nameof(cancellationProvider));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public void RegisterCallback(TimedCallback callback)
        {
            _callbacks.Add(callback);
        }

        public bool RemoveCallback(string callbackId)
        {
            var existingCallback = _callbacks.FirstOrDefault(c => c.Id == callbackId);

            if (existingCallback != null)
            {
                _callbacks.Remove(existingCallback);

                return true;
            }

            return false;
        }

        public void RemoveAllCallbacks()
        {
            _callbacks.Clear();
        }

        public void Start()
        {
            _cancellationProvider.Prepare();

            _timingTask = Task.Run(RunLoop);
        }

        public void Stop()
        {
            _cancellationProvider.Cancel();
        }

        private async Task RunLoop()
        {
            var currentTime = _timeProvider.GetCurrentTime();

            if (_lastExecutionTime == default)
            {
                _lastExecutionTime = currentTime;
            }

            if (currentTime.Second > 1)
            {
                InvokeDueCallbacks(currentTime);

                await CatchUpToNextMinute(currentTime);
            }

            while (_cancellationProvider.IsExecuting)
            {
                currentTime = _timeProvider.GetCurrentTime();

                if (currentTime.Day != _lastExecutionTime.Day)
                {
                    ResetDailyExecutionState();
                }

                InvokeDueCallbacks(currentTime);

                await Task.Delay(_timeProvider.GetClockInterval(), _cancellationProvider.Token);

                _lastExecutionTime = currentTime;
            }
        }

        private async Task CatchUpToNextMinute(DateTime currentTime)
        {
            var nextMinute = currentTime.AddMinutes(1);

            await Task.Run(async () =>
            {
                while (currentTime.Minute != nextMinute.Minute)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));

                    currentTime = _timeProvider.GetCurrentTime();
                }
            });
        }

        private void InvokeDueCallbacks(DateTime currentTime)
        {
            foreach (var callback in _callbacks)
            {
                if (callback.IsDue(currentTime))
                {
                    callback.Invoke();
                }
            }
        }

        private void ResetDailyExecutionState()
        {
            foreach (var callback in _callbacks)
            {
                callback.ResetDailyExecutionState();
            }
        }

        public void Dispose()
        {
            _cancellationProvider?.Dispose();

            if (_timingTask != null)
            {
                if (_timingTask.IsCompleted)
                {
                    return;
                }

                _timingTask.Wait();
                _timingTask.Dispose();
            }
        }
    }
}
