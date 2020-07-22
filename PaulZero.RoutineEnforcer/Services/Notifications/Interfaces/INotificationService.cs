using System;
using System.Threading.Tasks;

namespace PaulZero.RoutineEnforcer.Services.Notifications.Interfaces
{
    internal interface INotificationService
    {
        void CancelToastNotification(Guid id);

        Task ShowCountdownNotificationAsync(string title, string message, string progressStatus, string skipButtonLabel, TimeSpan delay);
    }
}
