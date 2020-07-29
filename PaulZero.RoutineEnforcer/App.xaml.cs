using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using PaulZero.RoutineEnforcer.Services;
using PaulZero.RoutineEnforcer.Services.Clock;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using PaulZero.RoutineEnforcer.Services.ComputerControl;
using PaulZero.RoutineEnforcer.Services.ComputerControl.Interfaces;
using PaulZero.RoutineEnforcer.Services.Config;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using PaulZero.RoutineEnforcer.Services.Notifications;
using PaulZero.RoutineEnforcer.Services.Notifications.Interfaces;
using PaulZero.RoutineEnforcer.Services.Routine;
using PaulZero.RoutineEnforcer.Services.Routine.Interfaces;
using PaulZero.RoutineEnforcer.Views.Models.Windows;
using PaulZero.RoutineEnforcer.Views.Windows;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace PaulZero.RoutineEnforcer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider AppServices { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigureToastNotifications();
            ConfigureServices();
            EnsureProgramDataDirectoryExists();

            var routineService = AppServices.GetService<IRoutineService>();

            routineService.Start();

            AppServices.GetService<MainWindow>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppServices.Dispose();

            base.OnExit(e);
        }

        private void EnsureProgramDataDirectoryExists()
        {
            var appDataDirectory = PathUtilities.GetProgramDataDirectory();

            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }
        }

        private void ConfigureToastNotifications()
        {
            DesktopNotificationManagerCompat.RegisterAumidAndComServer<ToastNotificationActivator>("PaulZero.WindowsRoutine");

            DesktopNotificationManagerCompat.RegisterActivator<ToastNotificationActivator>();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Add logging and all core services.

            services
                .AddRoutineEnforcerLogging()
                .AddSingleton<INotificationService, NotificationService>()
                .AddSingleton<IConfigService, ConfigService>()
                .AddSingleton<IComputerControlService, ComputerControlService>()
                .AddSingleton<IClockService, ClockService>()
                .AddSingleton<IRoutineService, RoutineService>();

            // Add view models
            // TODO: Refactor existing view models to work this way...

            services.AddTransient<ManageScheduleWindowViewModel>();

            // Add windows
            // TODO: Refactor existing windows to take their view models via DI...            

            services.AddSingleton<MainWindow>();
            services.AddTransient<ManageScheduleWindow>();

            AppServices = services.BuildServiceProvider();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var hasBeenAddedToLog = false;

            try
            {
                var logger = AppServices?.GetService<ILogger<App>>();

                if (logger != null)
                {
                    logger.LogError(e.Exception, $"An unhandled exception occurred: {e.Exception.Message}{Environment.NewLine}{e.Exception.StackTrace}");

                    hasBeenAddedToLog = true;
                }
            }
            catch
            {
            }

            try
            {
                var errorWindow = new ErrorWindow("An unhandled exception occurred", default, e.Exception);

                errorWindow.ShowDialog();
            }
            catch
            {
                var errorBuilder = new StringBuilder();

                errorBuilder
                    .AppendLine("An unhandled exception has occurred, please note this down and pass it on to the developer:")
                    .AppendLine();

                if (hasBeenAddedToLog)
                {
                    errorBuilder.AppendLine("This should also have been appended to the application log.")
                        .AppendLine();
                }

                errorBuilder
                    .AppendLine(e.Exception.Message)
                    .AppendLine()
                    .AppendLine(e.Exception.StackTrace);

                MessageBox.Show(errorBuilder.ToString(), "Unhandled Exception", MessageBoxButton.OK,
                    MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }

            e.Handled = true;
        }
    }
}
