using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using PaulZero.WindowsRoutine.Wpf.Services.Notifications;
using System;
using System.Runtime.InteropServices;

namespace PaulZero.WindowsRoutine.Wpf
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("7490a6b4-7d4d-46aa-a341-10e48a50c5c4")]
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
