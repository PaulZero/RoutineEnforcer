﻿using Microsoft.Extensions.Logging;
using System;

namespace PaulZero.RoutineEnforcer.Services.Clock.Interfaces
{
    public interface ITimedCallback
    {
        TimedCallbackExecutionState DailyExecutionState { get; }

        string Id { get; }

        TimedCallbackExecutionState LastExecutionState { get; }

        void Invoke(ILogger logger);

        bool IsDue(DateTime currentDateTime);

        void ResetDailyExecutionState(ILogger logger);
    }
}