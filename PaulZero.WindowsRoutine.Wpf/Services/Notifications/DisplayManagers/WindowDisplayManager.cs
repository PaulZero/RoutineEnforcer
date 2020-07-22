using Microsoft.Extensions.DependencyInjection;
using PaulZero.WindowsRoutine.Wpf.Models.View.Window;
using PaulZero.WindowsRoutine.Wpf.Windows;
using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications.DisplayManagers
{
    class WindowDisplayManager : AbstractDisplayManager
    {
        private readonly FallbackNotificationWindow _window;
        private readonly FallbackNotificationWindowViewModel _windowViewModel;

        public WindowDisplayManager(Guid notificationId, string title, string message, string progressStatusText, string skipButtonText)
            : base(notificationId, title, message, progressStatusText, skipButtonText)
        {
            _windowViewModel = new FallbackNotificationWindowViewModel(title, message, progressStatusText, skipButtonText);
            _window = new FallbackNotificationWindow(_windowViewModel);
        }

        public override void Hide()
        {
            _window.Hide();
        }

        public override void Show(DisplayManagerUpdateData initialData)
        {
            _windowViewModel.PercentageProgress = initialData.PercentageProgress;
            _windowViewModel.RemainingTime = initialData.RemainingTime;

            _window.Closing += OnWindowClosing;

            _window.Show();
        }

        public override void Update(DisplayManagerUpdateData updateData)
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
