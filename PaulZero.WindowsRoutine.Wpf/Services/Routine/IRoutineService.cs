using PaulZero.WindowsRoutine.Wpf.Models.View;
using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Routine
{
    internal interface IRoutineService
    {
        TimeSpan GetNextWarningCountdown();

        ScheduledEventViewModel[] GetTaskOverview();

        void Start();
    }
}