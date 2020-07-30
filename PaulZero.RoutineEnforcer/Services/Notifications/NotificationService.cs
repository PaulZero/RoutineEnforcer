using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using PaulZero.RoutineEnforcer.Services.Notifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace PaulZero.RoutineEnforcer.Services.Notifications
{
    internal class NotificationService : INotificationService
    {
        private bool ForceWindowStyleNotifications => _configService.GetAppConfiguration().Options.ForceWindowStyleNotifications;

        private readonly IConfigService _configService;
        private readonly Dictionary<Guid, Notification> _currentNotifications = new Dictionary<Guid, Notification>();
        private readonly ILogger _logger;
        private readonly ToastNotifier _toastNotifier;

        public NotificationService(ILogger<INotificationService> logger, IConfigService configService)
        {
            _configService = configService;
            _logger = logger;
            _toastNotifier = DesktopNotificationManagerCompat.CreateToastNotifier();
        }

        public void AbortAllNotifications()
        {
            var notificationIds = _currentNotifications.Keys.ToArray();

            foreach (var id in notificationIds)
            {
                var notification = _currentNotifications[id];

                notification.Abort();
                notification.Dispose();

                _currentNotifications.Remove(id);
            }
        }

        public void SkipToastNotification(Guid id)
        {
            _logger.LogDebug($"Skip request received for notification {id}.");

            _currentNotifications.TryGetValue(id, out var notification);

            if (notification == null)
            {
                _logger.LogDebug($"Unable to find notification {id} to skip!");

                return;
            }

            _currentNotifications.Remove(id);

            notification.Skip();
        }

        public async Task<NotificationResult> ShowCountdownNotificationAsync(string title, string message, string progressStatus, string skipButtonLabel, TimeSpan delay)
        {
            var result = NotificationResult.ServiceFailed;

            try
            {
                using var notification = new Notification(title, message, progressStatus, skipButtonLabel, delay, _toastNotifier, ForceWindowStyleNotifications);

                _currentNotifications.Add(notification.Id, notification);

                result = await notification.ShowAsync();

                if (_currentNotifications.ContainsKey(notification.Id))
                {
                    _currentNotifications.Remove(notification.Id);
                }

                if (result == NotificationResult.Failed && !string.IsNullOrWhiteSpace(notification.FailureMessage))
                {
                    _logger.LogError($"Notification returned failed result, with a message of {notification.FailureMessage}");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"An error occurred showing countdown notification (result was {result}): {exception.Message}");
            }

            return result;
        }
    }
}
