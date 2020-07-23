using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Views.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaulZero.RoutineEnforcer.Views.Models.Controls
{
    public class TaskSummaryViewModel : AbstractViewModel
    {
        public IEnumerable<NoComputerPeriodViewModel> NoComputerPeriods
        {
            get => _noComputerPeriods;
            set
            {
                _noComputerPeriods = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HasNoComputerPeriods));
                NotifyPropertyChanged(nameof(HasConfiguredTasks));
            }
        }

        public IEnumerable<ScheduledEventViewModel> ScheduledEvents
        {
            get => _scheduledEvents;
            set
            {
                _scheduledEvents = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HasScheduledEvents));
                NotifyPropertyChanged(nameof(HasConfiguredTasks));
            }
        }

        public bool HasNoComputerPeriods => NoComputerPeriods?.Any() ?? false;

        public bool HasScheduledEvents => ScheduledEvents?.Any() ?? false;

        public bool HasConfiguredTasks => HasNoComputerPeriods || HasScheduledEvents;

        private IEnumerable<NoComputerPeriodViewModel> _noComputerPeriods;
        private IEnumerable<ScheduledEventViewModel> _scheduledEvents;

        public TaskSummaryViewModel()
            : this(CreateDebugNoComputerPeriodViewModels(), CreateDebugScheduledEventViewModels())
        {   
        }

        public TaskSummaryViewModel(IEnumerable<NoComputerPeriodViewModel> noComputerPeriods, IEnumerable<ScheduledEventViewModel> scheduledEvents)
        {
            NoComputerPeriods = noComputerPeriods.OrderBy(p => p.NextDueDate);
            ScheduledEvents = scheduledEvents.OrderBy(s => s.NextDueDate);
        }

        public TaskSummaryViewModel(IEnumerable<NoComputerPeriod> noComputerPeriods, IEnumerable<ScheduledEvent> scheduledEvents)
            : this(noComputerPeriods.Select(p => new NoComputerPeriodViewModel(p)), scheduledEvents.Select(s => new ScheduledEventViewModel(s)))
        {
        }

        private static IEnumerable<NoComputerPeriodViewModel> CreateDebugNoComputerPeriodViewModels()
        {
            var noComputerPeriods = new[]
            {
                new NoComputerPeriod
                {
                    Name = "You should be sleeping",
                    StartTime = new TimeSpan(23, 0, 0),
                    EndTime = new TimeSpan(7, 0, 0),
                    DaysActive = DaySelection.Daily,
                    ActionDelay = TimeSpan.FromMinutes(15)
                },
                new NoComputerPeriod
                {
                    Name = "You should be out farming pigeons for the pot",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    DaysActive = DaySelection.Daily,
                    ActionDelay = TimeSpan.FromMinutes(15)
                }
            };

            return noComputerPeriods.Select(p => new NoComputerPeriodViewModel(p));
        }

        private static IEnumerable<ScheduledEventViewModel> CreateDebugScheduledEventViewModels()
        {
            var scheduledEvents = new[]
            {
                new ScheduledEvent
                {
                    Name = "Eat some lunch",
                    WarningTime = new TimeSpan(12, 0, 0),
                    ActionDelay = TimeSpan.FromMinutes(10),
                    ActionType = EventActionType.LockScreen,
                    DaysScheduled = DaySelection.Daily
                },
                new ScheduledEvent
                {
                    Name = "Buy a cat",
                    WarningTime = DateTime.Now.AddHours(1).TimeOfDay,
                    ActionDelay = TimeSpan.FromMinutes(10),
                    ActionType = EventActionType.SleepComputer,
                    DaysScheduled = DaySelection.Daily
                }
            };

            return scheduledEvents.Select(s => new ScheduledEventViewModel(s));
        }
    }
}
