using System;
using System.Threading.Tasks;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications
{
    internal interface INotificationService
    {
        string StatusMessage { get; }

        bool CanShowNotifications { get; }

        void CancelToastNotification(Guid id);

        //void ShowNotification(string id, string title, string message, params (string buttonLabel, string buttonArguments)[] buttons);

        Task ShowCountdownNotificationAsync(Guid id, string title, string message, string progressStatus, string skipButtonLabel, TimeSpan delay);
    }
}
