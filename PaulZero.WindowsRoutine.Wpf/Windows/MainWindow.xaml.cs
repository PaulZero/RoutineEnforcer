using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using PaulZero.WindowsRoutine.Wpf.Services.Config;
using PaulZero.WindowsRoutine.Wpf.Services.Notifications;
using PaulZero.WindowsRoutine.Wpf.Services.Routine;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PaulZero.WindowsRoutine.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool HasLoadingError
        {
            get => _hasLoadingError;
            set
            {
                _hasLoadingError = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasLoadingError)));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }

        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _hasLoadingError;
        private string _statusMessage = "Loading...";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var imageStream = new MemoryStream(Resource.clock_icon);
            var notificationIcon = new Icon(imageStream);

            imageStream.Position = 0;

            var wpfIcon = new BitmapImage();
            wpfIcon.BeginInit();
            wpfIcon.StreamSource = imageStream;
            wpfIcon.EndInit();

            Icon = wpfIcon;
            TaskbarIcon.Icon = notificationIcon;

            var notificationService = App.AppServices.GetService<INotificationService>();

            if (!notificationService.CanShowNotifications)
            {
                HasLoadingError = true;

                StatusMessage = notificationService.StatusMessage;

                return;
            }

            var routineService = App.AppServices.GetService<IRoutineService>();

            routineService.Start();

            Visibility = Visibility.Collapsed;
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ScheduleEvent_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateScheduledTaskWindow();

            if (window.ShowDialog() == true)
            {
                var configService = App.AppServices.GetService<IConfigService>();

                configService.CreateNewScheduledEvent(window.CreateScheduledEvent());
            }
        }
    }
}
