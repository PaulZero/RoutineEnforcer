using Microsoft.Extensions.DependencyInjection;
using PaulZero.RoutineEnforcer.Services.Notifications.Interfaces;
using PaulZero.RoutineEnforcer.Views.Models.Window;
using PaulZero.RoutineEnforcer.Views.Windows;
using System;

namespace PaulZero.RoutineEnforcer.Services.Notifications.NotificationViews
{
    internal class WindowNotificationView : AbstractNotificationView
    {
        private readonly NotificationWindow _window;
        private readonly NotificationWindowViewModel _windowViewModel;

        public WindowNotificationView(Guid notificationId, string title, string message, string progressStatusText, string skipButtonText)
            : base(notificationId, title, message, progressStatusText, skipButtonText)
        {
            _windowViewModel = new NotificationWindowViewModel(title, message, progressStatusText, skipButtonText);
            _window = new NotificationWindow(_windowViewModel);
        }

        public override void Hide()
        {
            _window.Hide();
        }

        public override void Show(NotificationUpdateData initialData)
        {
            _windowViewModel.PercentageProgress = initialData.PercentageProgress;
            _windowViewModel.RemainingTime = initialData.RemainingTime;

            _window.Closing += OnWindowClosing;

            _window.Show();
        }

        public override void Update(NotificationUpdateData updateData)
        {
            _windowViewModel.PercentageProgress = updateData.PercentageProgress;
            _windowViewModel.RemainingTime = updateData.RemainingTime;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _window.Closing -= OnWindowClosing;

            App.AppServices.GetService<INotificationService>().CancelToastNotification(NotificationId);
        }
    }
}
