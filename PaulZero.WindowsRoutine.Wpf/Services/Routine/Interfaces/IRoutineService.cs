using PaulZero.RoutineEnforcer.Views.Models;
using System;

namespace PaulZero.RoutineEnforcer.Services.Routine.Interfaces
{
    internal interface IRoutineService
    {
        TimeSpan GetNextWarningCountdown();

        ScheduledEventViewModel[] GetTaskOverview();

        void Start();
    }
}