using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using PaulZero.WindowsRoutine.Wpf.Services.Actions;
using PaulZero.WindowsRoutine.Wpf.Services.Clock;
using PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces;
using PaulZero.WindowsRoutine.Wpf.Services.Config;
using PaulZero.WindowsRoutine.Wpf.Services.Notifications;
using PaulZero.WindowsRoutine.Wpf.Services.Routine;
using System.Windows;

namespace PaulZero.WindowsRoutine.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider AppServices { get; private set; }

        public App()
        {
            ConfigureToastNotifications();
            ConfigureServices();
        }

        private void ConfigureToastNotifications()
        {
            DesktopNotificationManagerCompat.RegisterAumidAndComServer<ToastNotificationActivator>("PaulZero.WindowsRoutine");

            DesktopNotificationManagerCompat.RegisterActivator<ToastNotificationActivator>();

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IActionService, ActionService>();
            services.AddSingleton<IClockService, ClockService>();

            services.AddSingleton<IRoutineService, RoutineService>();

            AppServices = services.BuildServiceProvider();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            AppServices.Dispose();
        }
    }
}
