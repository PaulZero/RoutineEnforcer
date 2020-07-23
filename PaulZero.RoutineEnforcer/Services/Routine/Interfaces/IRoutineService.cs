using PaulZero.RoutineEnforcer.Views.Models;

namespace PaulZero.RoutineEnforcer.Services.Routine.Interfaces
{
    internal interface IRoutineService
    {
        ScheduledEventViewModel[] GetTaskOverview();

        void Start();
    }
}