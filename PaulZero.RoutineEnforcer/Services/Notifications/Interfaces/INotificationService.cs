using System;
using System.Threading.Tasks;

namespace PaulZero.RoutineEnforcer.Services.Notifications.Interfaces
{
    internal interface INotificationService
    {
        void AbortAllNotifications();

        void SkipToastNotification(Guid id);

        Task<NotificationResult> ShowCountdownNotificationAsync(string title, string message, string progressStatus, string skipButtonLabel, TimeSpan delay);
    }
}
