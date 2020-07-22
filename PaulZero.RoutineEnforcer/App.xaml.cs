﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
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

            services.AddLogging(o =>
            {
                var appDataDirectory = PathUtilities.GetProgramDataDirectory();

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
            services.AddSingleton<IComputerControlService, ComputerControlService>();
            services.AddSingleton<IClockService, ClockService>();
            services.AddSingleton<IRoutineService, RoutineService>();

            AppServices = services.BuildServiceProvider();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            AppServices.Dispose();
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConfigureToastNotifications();
            ConfigureServices();
            EnsureProgramDataDirectoryExists();

            var routineService = AppServices.GetService<IRoutineService>();

            routineService.Start();
        }
    }
}