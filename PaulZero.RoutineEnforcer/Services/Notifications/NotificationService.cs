using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using PaulZero.RoutineEnforcer.Services.Notifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace PaulZero.RoutineEnforcer.Services.Notifications
{
    internal class NotificationService : INotificationService
    {
        private readonly Dictionary<Guid, Notification> _currentNotifications = new Dictionary<Guid, Notification>();
        private readonly ILogger _logger;
        private readonly ToastNotifier _toastNotifier;

        public NotificationService(ILogger<INotificationService> logger)
        {
            _logger = logger;
            _toastNotifier = DesktopNotificationManagerCompat.CreateToastNotifier();
        }

        public void CancelToastNotification(Guid id)
        {
            if (_currentNotifications.TryGetValue(id, out var notification))
            {
                _currentNotifications.Remove(id);

                notification.Skip();
            }
        }

        public async Task ShowCountdownNotificationAsync(string title, string message, string progressStatus, string skipButtonLabel, TimeSpan delay)
        {
            var notification = new Notification(title, message, progressStatus, skipButtonLabel, delay, _toastNotifier);

            _currentNotifications.Add(notification.Id, notification);

            await notification.ShowAsync();

            if (_currentNotifications.ContainsKey(notification.Id))
            {
                _currentNotifications.Remove(notification.Id);
            }
        }
    }
}
