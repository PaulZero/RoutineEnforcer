using PaulZero.RoutineEnforcer.Views.Models.Controls;

namespace PaulZero.RoutineEnforcer.Services.Routine.Interfaces
{
    internal interface IRoutineService
    {
        TaskSummaryViewModel GetTaskOverview();

        void Start();
    }
}