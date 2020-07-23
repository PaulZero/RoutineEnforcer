using Microsoft.Extensions.Logging;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using System;

namespace PaulZero.RoutineEnforcer.Services.Clock
{
    public abstract class AbstractTimedCallback : ITimedCallback
    {
        public string Id { get; }

        public TimedCallbackExecutionState DailyExecutionState { get; private set; } = TimedCallbackExecutionState.HasNotRun;

        public TimedCallbackExecutionState LastExecutionState { get; private set; } = TimedCallbackExecutionState.HasNotRun;

        public abstract bool IsPeriod { get; }

        public bool IsExecuting { get; private set; }

        private readonly Action<ITimedCallback> _callback;

        public AbstractTimedCallback(Action<ITimedCallback> callback)
        {
            Id = Guid.NewGuid().ToString();

            _callback = callback ?? throw new ArgumentNullException(nameof(callback), "Callback given must not be null");
        }

        public abstract bool IsDue(DateTime currentDateTime);

        public void Invoke(ILogger logger)
        {
            try
            {
                IsExecuting = true;

                logger.LogDebug($"Invoking callback '{Id}'");

                _callback.Invoke(this);

                DailyExecutionState = TimedCallbackExecutionState.RanSuccessfully;
                LastExecutionState = TimedCallbackExecutionState.RanSuccessfully;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Failed to invoke callback '{Id}'.");

                DailyExecutionState = TimedCallbackExecutionState.FailedToRun;
                LastExecutionState = TimedCallbackExecutionState.FailedToRun;
            }
        }

        public void ResetDailyExecutionState(ILogger logger)
        {
            logger.LogDebug($"Resetting daily execution state for callback '{Id}'.");

            DailyExecutionState = TimedCallbackExecutionState.HasNotRun;
        }

        public void MarkAsFinished()
        {
            IsExecuting = false;
        }
    }
}
