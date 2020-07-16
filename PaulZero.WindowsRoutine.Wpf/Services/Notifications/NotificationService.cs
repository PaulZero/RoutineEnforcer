using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications
{
    internal class NotificationService : INotificationService
    {
        internal const string NotificationGroup = "0D78329D-D8EB-4BF7-97CF-5D5DE65B2BD1";

        public string StatusMessage => CreateStatusMessage();

        public bool CanShowNotifications => _toastNotifier.Setting == NotificationSetting.Enabled;

        private readonly ToastNotifier _toastNotifier;

        private readonly IDictionary<string, ToastNotification> _currentNotifications = new Dictionary<string, ToastNotification>();
        private readonly ILogger _logger;
        private readonly IList<string> _toastsRequiringCancellation = new List<string>();

        public NotificationService(ILogger<INotificationService> logger)
        {
            _logger = logger;
            _toastNotifier = DesktopNotificationManagerCompat.CreateToastNotifier();
        }

        //public void ShowNotification(string id, string title, string message, params (string buttonLabel, string buttonArguments)[] buttons)
        //{
        //    if (!CanShowNotifications)
        //    {
        //        throw new Exception($"Cannot show a notification: {StatusMessage}");
        //    }

        //    var builder = new ToastContentBuilder()
        //        .AddToastActivationInfo(default, ToastActivationType.Foreground)
        //        .AddHeader(CreateHeaderId(id), title, CreateHeaderArgument(id))
        //        .AddText(message);

        //    if (buttons?.Any() ?? false)
        //    {
        //        foreach ((var buttonLabel, var buttonArgument) in buttons)
        //        {
        //            builder.AddButton(buttonLabel, ToastActivationType.Foreground, buttonArgument);
        //        }
        //    }

        //    // And create the toast notification
        //    var toast = new ToastNotification(builder.GetToastContent().GetXml());

        //    // And then show it
        //    DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
        //}

        public void CancelToastNotification(Guid id)
        {
            _logger.LogDebug($"Flagging notification with ID '{id}' for cancellation.");

            _toastsRequiringCancellation.Add(id.ToString());
        }

        public async Task ShowCountdownNotificationAsync(Guid id, string title, string message, string progressStatus, string skipButtonLabel, TimeSpan delay)
        {
            var toastId = id.ToString();

            if (!CanShowNotifications)
            {
                throw new Exception($"Cannot show a notification: {StatusMessage}");
            }

            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            var iconPath = Path.Combine(assemblyDirectory, "clock_icon.ico");

            var toastContent = new ToastContent
            {
                ActivationType = ToastActivationType.Foreground,
                Scenario = ToastScenario.Alarm,
                Audio = new ToastAudio
                {
                    Loop = false
                },
                Header = new ToastHeader(CreateHeaderId(toastId), title, CreateHeaderArgument(toastId)),
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = message,
                            },
                            new AdaptiveProgressBar
                            {
                                Status = progressStatus,
                                Value = new BindableProgressBarValue("ProgressBarValue"),
                                ValueStringOverride = new BindableString("ProgressBarValueString")
                            },
                        },
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = iconPath,
                        },
                    },
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton(skipButtonLabel, CreateSkipCountdownArgument(toastId))
                    }
                }
            };

            var initialDataValues = new Dictionary<string, string>
            {
                ["ProgressBarValue"] = "1",
                ["ProgressBarValueString"] = string.Format("{0:00}:{1:00}:{2:00}", delay.Hours, delay.Minutes, delay.Seconds)
            };

            var toast = new ToastNotification(toastContent.GetXml())
            {
                Tag = toastId,

                Group = NotificationGroup,
                Data = new NotificationData(initialDataValues)
                {
                    SequenceNumber = 1
                }
            };

            _currentNotifications.Add(toastId, toast);

            toast.Failed += Toast_Failed;
            toast.Dismissed += Toast_Dismissed;

            _toastNotifier.Show(toast);

            for (var i = 1; i <= delay.TotalSeconds; i++)
            {
                if (_toastsRequiringCancellation.Contains(toastId))
                {
                    _toastsRequiringCancellation.Remove(toastId);

                    break;
                }

                var remaining = delay.Subtract(TimeSpan.FromSeconds(i));

                var percentageProgress = Math.Round((100 / delay.TotalSeconds) * remaining.TotalSeconds);
                var percentageFraction = percentageProgress / 100;

                await Task.Delay(TimeSpan.FromSeconds(1));

                var updatedValues = new Dictionary<string, string>
                {
                    ["ProgressBarValue"] = string.Format("{0:N2}", percentageFraction),
                    ["ProgressBarValueString"] = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds)
                };

                var updatedData = new NotificationData(updatedValues, (uint)(i + 1));

                var updateResult = _toastNotifier.Update(updatedData, toastId, NotificationGroup);

                if (updateResult != NotificationUpdateResult.Succeeded)
                {
                    if (updateResult == NotificationUpdateResult.Failed)
                    {
                        _logger.LogError($"Failed to update notification '{toastId}'");
                    }

                    return;
                }
            }

            _toastNotifier.Hide(toast);
        }

        private void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            if (_currentNotifications.ContainsKey(sender.Tag))
            {
                _currentNotifications.Remove(sender.Tag);
            }
        }

        private void Toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            if (_currentNotifications.ContainsKey(sender.Tag))
            {
                _currentNotifications.Remove(sender.Tag);
            }
        }

        private string CreateStatusMessage()
            => _toastNotifier.Setting switch
            {
                NotificationSetting.DisabledByGroupPolicy => "Cannot send notifications as they have been disabled via group policy.",
                NotificationSetting.DisabledByManifest => "This app was poorly written and doesn't even request permission to send notifications somehow.",
                NotificationSetting.DisabledForApplication => "You have disabled notifications for this app, please enable them to use it.",
                NotificationSetting.DisabledForUser => "You have disabled notifications for your account, please turn them back on to use this app.",
                NotificationSetting.Enabled => "Notifications are enabled, the app will run properly.",
                _ => "Some weird unknown status was returned by Windows, suffice to say this is broken.",
            };

        private string CreateSkipCountdownArgument(string id)
            => $"{id}:skip";

        private string CreateHeaderId(string id)
            => $"{id}:header";

        private string CreateHeaderArgument(string id)
            => $"{id}:header";
    }
}
