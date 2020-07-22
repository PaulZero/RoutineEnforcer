using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Windows.UI.Notifications;

namespace PaulZero.RoutineEnforcer.Services.Notifications.NotificationViews
{
    internal class ToastNotificationView : AbstractNotificationView
    {
        internal const string NotificationGroup = "0d78329d-d8eb-4bf7-97cf-5d5de65b2bd1";

        private ToastNotification _toast;
        private readonly ToastNotifier _toastNotifier;
        private DateTime _toastShownAt;

        public ToastNotificationView(ToastNotifier toastNotifier, Guid notificationId, string title, string message, string progressStatusText, string skipButtonText)
            : base(notificationId, title, message, progressStatusText, skipButtonText)
        {
            _toastNotifier = toastNotifier;
        }

        public override void Hide()
        {
            _toastNotifier.Hide(_toast);
        }

        public override void Show(NotificationUpdateData initialData)
        {
            var toastId = NotificationId.ToString();

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
                Header = new ToastHeader($"header:{toastId}", Title, ""),
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = Message,
                            },
                            new AdaptiveProgressBar
                            {
                                Status = ProgressStatusText,
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
                        new ToastButton(SkipButtonText, CreateSkipCountdownArgument(toastId))
                    }
                }
            };

            var initialDataValues = new Dictionary<string, string>
            {
                ["ProgressBarValue"] = "1",
                ["ProgressBarValueString"] = initialData.RemainingTime
            };

            _toast = new ToastNotification(toastContent.GetXml())
            {
                Tag = toastId,
                Group = NotificationGroup,
                Data = new NotificationData(initialDataValues)
                {
                    SequenceNumber = initialData.UpdateNumber
                }
            };

            _toast.Failed += (s, e) =>
            {
                MarkAsFailed();
            };

            _toast.Dismissed += (s, e) =>
            {
                if (_toastShownAt.AddSeconds(2) > DateTime.Now)
                {
                    MarkAsFailed();
                }
            };

            _toastShownAt = DateTime.Now;
            _toastNotifier.Show(_toast);
        }

        public override void Update(NotificationUpdateData updateData)
        {
            var updatedValues = new Dictionary<string, string>
            {
                ["ProgressBarValue"] = updateData.FractionProgress,
                ["ProgressBarValueString"] = updateData.RemainingTime
            };

            var updatedData = new NotificationData(updatedValues, updateData.UpdateNumber);

            _toastNotifier.Update(updatedData, _toast.Tag, NotificationGroup);
        }

        private string CreateSkipCountdownArgument(string id)
            => $"{id}:skip";
    }
}
