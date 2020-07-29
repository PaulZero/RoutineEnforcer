using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using PaulZero.RoutineEnforcer.Views.Commands;
using PaulZero.RoutineEnforcer.Views.Dialogs;
using PaulZero.RoutineEnforcer.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PaulZero.RoutineEnforcer.Views.Models.Windows
{
    public class ManageScheduleWindowViewModel
    {
        public ObservableCollection<NoComputerPeriodViewModel> NoComputerPeriods { get; private set; }

        public ObservableCollection<ScheduledEventViewModel> ScheduledEvents { get; private set; }

        public ICommand CreateNoComputerPeriodCommand { get; private set; }

        public ICommand CreateScheduledEventCommand { get; private set; }

        public ICommand DeleteNoComputerPeriodCommand { get; private set; }

        public ICommand DeleteScheduledEventCommand { get; private set; }

        public ICommand EditNoComputerPeriodCommand { get; private set; }

        public ICommand EditScheduledEventCommand { get; private set; }

        private IConfigService _configService;

        public ManageScheduleWindowViewModel()
        {
            var noComputerPeriods = new List<NoComputerPeriod>();
            var scheduledEvents = new List<ScheduledEvent>();

#if DEBUG
            noComputerPeriods.AddRange(new[]
            {
                new NoComputerPeriod
                {
                    Name = "Do not ever be a gremlin",
                    StartTime = new TimeSpan(22, 0, 0),
                    EndTime = new TimeSpan(8, 0, 0),
                    ActionDelay = TimeSpan.FromMinutes(10),
                    DaysActive = DaySelection.Daily
                }
            });

            scheduledEvents.AddRange(new[]
            {
                new ScheduledEvent
                {
                    Name = "Eat some lunch",
                    WarningTime = new TimeSpan(12, 0, 0),
                    ActionDelay = TimeSpan.FromMinutes(15),
                    ActionType = EventActionType.LockScreen,
                    DaysScheduled = DaySelection.Daily
                }
            });
#endif

            Initialise(noComputerPeriods, scheduledEvents);
        }

        public ManageScheduleWindowViewModel(IConfigService configService)
        {
            _configService = configService;

            var config = _configService.GetAppConfiguration();

            Initialise(config.NoComputerPeriods, config.ScheduledEvents);
        }

        private void Initialise(IEnumerable<NoComputerPeriod> noComputerPeriods, IEnumerable<ScheduledEvent> scheduledEvents)
        {
            CreateNoComputerPeriodCommand = new CallbackCommand(CreateNoComputerPeriod);
            CreateScheduledEventCommand = new CallbackCommand(CreateScheduledEvent);
            DeleteNoComputerPeriodCommand = new CallbackCommand(DeleteNoComputerPeriod);
            DeleteScheduledEventCommand = new CallbackCommand(DeleteScheduledEvent);
            EditNoComputerPeriodCommand = new CallbackCommand(EditNoComputerPeriod);
            EditScheduledEventCommand = new CallbackCommand(EditScheduledEvent);

            NoComputerPeriods = new ObservableCollection<NoComputerPeriodViewModel>(noComputerPeriods.Select(p => new NoComputerPeriodViewModel(p)));
            ScheduledEvents = new ObservableCollection<ScheduledEventViewModel>(scheduledEvents.Select(e => new ScheduledEventViewModel(e)));
        }

        private void CreateNoComputerPeriod(object parameter = null)
        {
            var window = new ScheduleNoComputerPeriodWindow();

            if (window.ShowDialog() == true)
            {
                var noComputerPeriod = window.CreateNoComputerPeriod();

                _configService.CreateNewNoComputerPeriod(noComputerPeriod);

                NoComputerPeriods.Add(new NoComputerPeriodViewModel(noComputerPeriod));
            }
        }

        private void CreateScheduledEvent(object parameter = null)
        {
            var window = new ScheduleEventWindow();

            if (window.ShowDialog() == true)
            {
                var scheduledEvent = window.CreateScheduledEvent();

                _configService.CreateNewScheduledEvent(scheduledEvent);

                ScheduledEvents.Add(new ScheduledEventViewModel(scheduledEvent));
            }
        }

        private void DeleteNoComputerPeriod(object periodId)
        {
            var noComputerPeriod = NoComputerPeriods.FirstOrDefault(p => p.Id == periodId?.ToString());

            if (noComputerPeriod == null)
            {
                return;
            }

            var confirmation = ConfirmationDialog.ShowYesNoDialog(
                "Delete No Computer Period?",
                $"Are you sure you wish to delete the no computer period called '{noComputerPeriod.Name}'?");

            if (confirmation)
            {
                _configService.RemoveNoComputerPeriodById(noComputerPeriod.Id);

                NoComputerPeriods.Remove(noComputerPeriod);
            }
        }

        private void DeleteScheduledEvent(object eventId)
        {
            var scheduledEvent = ScheduledEvents.FirstOrDefault(e => e.Id == eventId?.ToString());

            if (scheduledEvent == null)
            {
                return;
            }

            var confirmation = ConfirmationDialog.ShowYesNoDialog(
                "Delete Scheduled Event?",
                $"Are you sure you wish to delete the scheduled event called '{scheduledEvent.Name}'?");

            if (confirmation)
            {
                _configService.RemoveScheduledEventById(scheduledEvent.Id);

                ScheduledEvents.Remove(scheduledEvent);
            }
        }

        private void EditNoComputerPeriod(object periodId)
        {
            var noComputerPeriod = NoComputerPeriods.FirstOrDefault(p => p.Id == periodId?.ToString());

            if (noComputerPeriod == null)
            {
                return;
            }

            var window = new ScheduleNoComputerPeriodWindow(noComputerPeriod);

            if (window.ShowDialog() == true)
            {
                var updatedNoComputerPeriod = window.CreateNoComputerPeriod();

                _configService.UpdateNoComputerPeriod(updatedNoComputerPeriod);

                NoComputerPeriods.Remove(noComputerPeriod);
                NoComputerPeriods.Add(new NoComputerPeriodViewModel(updatedNoComputerPeriod));
            }
        }

        private void EditScheduledEvent(object eventId)
        {
            var scheduledEvent = ScheduledEvents.FirstOrDefault(e => e.Id == eventId?.ToString());
            var window = new ScheduleEventWindow(scheduledEvent);

            if (window.ShowDialog() == true)
            {
                var updatedScheduledEvent = window.CreateScheduledEvent();

                _configService.UpdateScheduledEvent(updatedScheduledEvent);

                ScheduledEvents.Remove(scheduledEvent);
                ScheduledEvents.Add(new ScheduledEventViewModel(updatedScheduledEvent));
            }
        }
    }
}
