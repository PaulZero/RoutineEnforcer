using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using PaulZero.WindowsRoutine.Wpf.Services.Actions;
using PaulZero.WindowsRoutine.Wpf.Services.Clock;
using PaulZero.WindowsRoutine.Wpf.Services.Clock.Interfaces;
using PaulZero.WindowsRoutine.Wpf.Services.Config;
using PaulZero.WindowsRoutine.Wpf.Services.Notifications;
using PaulZero.WindowsRoutine.Wpf.Services.Routine;
using System;
using System.IO;
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
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddLogging(o =>
            {
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var appDataDirectory = Path.Combine(programData, "PaulZero", "WindowsRoutine");

                o.ClearProviders();
                o.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

                var config = new LoggingConfiguration();
                var fileTarget = new FileTarget
                {
                    Name = "file",
                    FileName = Path.Combine(appDataDirectory, "RoutineEnforcer.log"),
                    FileNameKind = FilePathKind.Absolute,
                    ArchiveOldFileOnStartup = true,
                    AutoFlush = true,
                    ArchiveFileName = Path.Combine(appDataDirectory, "RoutineEnforcer.{#}.log"),
                    ArchiveFileKind = FilePathKind.Absolute,
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    ArchiveDateFormat = "yyyy-MM-dd",
                    ArchiveEvery = FileArchivePeriod.Day,
                    MaxArchiveFiles = 7
                };

                config.AddTarget(fileTarget);
                config.AddRuleForAllLevels(fileTarget);

                LogManager.Configuration = config;

                o.AddNLog(LogManager.Configuration);
            });

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
