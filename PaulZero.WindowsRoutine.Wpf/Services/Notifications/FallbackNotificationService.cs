using Microsoft.Extensions.Logging;
using PaulZero.WindowsRoutine.Wpf.Models.View;
using PaulZero.WindowsRoutine.Wpf.Windows;
using System;
using System.Threading.Tasks;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications
{
    internal class FallbackNotificationService : INotificationService
    {
        public string StatusMessage => "Using the fallback notification service.";

        public bool CanShowNotifications => true;

        private readonly ILogger _logger;

        public FallbackNotificationService(ILogger<INotificationService> logger)
        {
            _logger = logger;
        }

        public void CancelToastNotification(Guid id)
        {
            // Not needed for fallback notifications...
        }

        public async Task ShowCountdownNotificationAsync(Guid id, string title, string message, string progressStatus, string skipButtonLabel, TimeSpan delay)
        {
            using var window = new FallbackNotificationWindow(
                new FallbackNotificationWindowViewModel(title, message, progressStatus, skipButtonLabel, delay));

            window.Show();

            await window.WaitForCompletionAsync();

            window.Close();
        }
    }
}
