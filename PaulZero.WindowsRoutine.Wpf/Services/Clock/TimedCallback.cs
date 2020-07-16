using System;
using System.Collections.Generic;
using System.Text;

namespace PaulZero.WindowsRoutine.Wpf.Services.Clock
{
    public class TimedCallback
    {
        public string Id { get; }

        public TimedCallbackExecutionState DailyExecutionState { get; private set; } = TimedCallbackExecutionState.HasNotRun;

        public TimedCallbackExecutionState LastExecutionState { get; private set; } = TimedCallbackExecutionState.HasNotRun;

        private readonly Action _callback;
        private readonly int _hour;
        private readonly int _minute;

        public TimedCallback(Action callback, int hour, int minute)
        {
            Id = Guid.NewGuid().ToString();

            ValidateHour(hour);
            ValidateMinute(minute);

            _callback = callback ?? throw new ArgumentNullException(nameof(callback), "Callback given must not be null");
            _hour = hour;
            _minute = minute;
        }

        public virtual bool IsDue(DateTime currentDateTime)
        {
            if (currentDateTime.Hour == _hour && currentDateTime.Minute == _minute)
            {
                return true;
            }

            return false;
        }

        public void Invoke()
        {
            try
            {
                _callback.Invoke();

                DailyExecutionState = TimedCallbackExecutionState.RanSuccessfully;
                LastExecutionState = TimedCallbackExecutionState.RanSuccessfully;
            }
            catch
            {
                DailyExecutionState = TimedCallbackExecutionState.FailedToRun;
                LastExecutionState = TimedCallbackExecutionState.FailedToRun;
            }
        }

        public void ResetDailyExecutionState()
        {
            DailyExecutionState = TimedCallbackExecutionState.HasNotRun;
        }

        private void ValidateHour(int hour)
        {
            if (hour < 0 || hour > 23)
            {
                throw new ArgumentOutOfRangeException("Hour given must be a 24 hour value expressed as an integer between 0 and 23");
            }
        }

        private void ValidateMinute(int minute)
        {
            if (minute < 0 || minute > 59)
            {
                throw new ArgumentOutOfRangeException("Minute given must be expressed as an integer value between 0 and 23");
            }
        }
    }
}
