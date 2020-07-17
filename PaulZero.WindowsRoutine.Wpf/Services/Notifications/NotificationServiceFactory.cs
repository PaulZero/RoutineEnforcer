using Microsoft.Extensions.Logging;
using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications
{
    internal class NotificationServiceFactory
    {
        public static INotificationService Create(ILogger<INotificationService> logger)
        {
            try
            {
                return new NotificationService(logger);
            }
            catch (Exception exception)
            {
                // TODO: This is critical to the app, it needs to be able to show proper notifications. These errors are probably the most important to log and fix.

                var errorMessage = "Unable to create standard notification service, falling back to window based version.";

                logger.LogError(exception, errorMessage);

                return new FallbackNotificationService(logger);
            }
        }
    }
}
