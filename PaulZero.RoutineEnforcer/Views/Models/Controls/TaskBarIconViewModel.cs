using Microsoft.Extensions.DependencyInjection;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using PaulZero.RoutineEnforcer.Views.Commands;
using PaulZero.RoutineEnforcer.Views.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PaulZero.RoutineEnforcer.Views.Models.Controls
{
    public class TaskBarIconViewModel : AbstractViewModel
    {
        public ICommand EditConfigCommand { get; }

        public ICommand QuitCommand { get; }

        public ICommand ScheduleEventCommand { get; }

        public ICommand ViewReadmeCommand { get; }

        public TaskBarIconViewModel()
        {
            EditConfigCommand = new CallbackCommand(EditConfig);
            QuitCommand = new CallbackCommand(Quit);
            ScheduleEventCommand = new CallbackCommand(ScheduleEvent);
            ViewReadmeCommand = new CallbackCommand(ViewReadme);
        }

        private void EditConfig(object parameter = null)
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var appDataDirectory = Path.Combine(programData, "PaulZero", "WindowsRoutine");
            var configFilePath = Path.Combine(appDataDirectory, "config.json");

            if (!File.Exists(configFilePath))
            {
                MessageBox.Show(
                    "A config file has not yet been created, you will need to schedule some events first.",
                    "No Configuration File",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);

                return;
            }

            MessageBox.Show(
                    "You will need to restart the application if you make any changes to the config file, it is not " +
                    "recommended that you edit this file unless you understand JSON.",
                    "Configuration",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);

            Process.Start("notepad", configFilePath);
        }

        private void Quit(object parameter = null)
        {
            Environment.Exit(0);
        }

        private void ScheduleEvent(object parameter = null)
        {
            var window = new ScheduleEventWindow();

            if (window.ShowDialog() == true)
            {
                var configService = App.AppServices.GetService<IConfigService>();

                configService.CreateNewScheduledEvent(window.CreateScheduledEvent());
            }
        }

        private void ViewReadme(object parameter = null)
        {
            var readmePath = Path.Combine(Environment.CurrentDirectory, "Readme.md");

            if (File.Exists(readmePath))
            {
                Process.Start("notepad", readmePath);
            }
            else
            {
                MessageBox.Show(
                    "Sorry, the readme file could not be found, if you have a bug please find me on Twitter @PaulZer0",
                    "No Readme",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
