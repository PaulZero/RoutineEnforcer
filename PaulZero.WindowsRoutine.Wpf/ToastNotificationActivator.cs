using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using PaulZero.WindowsRoutine.Wpf.Services.Notifications;
using System;
using System.Runtime.InteropServices;

namespace PaulZero.WindowsRoutine.Wpf
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("3F1007CD-7EDE-41E2-8F30-2BD4C1A44FB1")]
    [ComVisible(true)]
    public class ToastNotificationActivator : NotificationActivator
    {
        public const string LockScreenArgument = "LockScreen";

        public override void OnActivated(string invokedArgs, NotificationUserInput userInput, string appUserModelId)
        {
            if (!invokedArgs.Contains(':'))
            {
                return;
            }

            var arguments = invokedArgs.Split(':');

            if (arguments.Length != 2)
            {
                return;
            }

            var idString = arguments[0];
            var command = arguments[1];

            if (Guid.TryParse(idString, out var toastId))
            {
                if (command == "skip")
                {
                    var service = App.AppServices.GetService<INotificationService>();

                    service.CancelToastNotification(toastId);
                }
            }
        }
    }
}
