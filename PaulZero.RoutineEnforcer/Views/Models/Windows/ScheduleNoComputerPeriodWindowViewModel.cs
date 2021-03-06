﻿using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Models.Validation;
using PaulZero.RoutineEnforcer.Views.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PaulZero.RoutineEnforcer.Views.Models.Windows
{
    public class ScheduleNoComputerPeriodWindowViewModel : AbstractViewModel
    {
        public event Action<bool> DialogResultSet;

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;

                    NotifyPropertyChanged();
                }
            }
        }

        public TimeSpan StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;

                    NotifyPropertyChanged();
                }
            }
        }

        public NoComputerPeriod GetNoComputerPeriod()
        {
            var noComputerPeriod = new NoComputerPeriod
            {
                Name = Name,
                StartTime = StartTime,
                EndTime = EndTime,
                DaysActive = DaysSelected,
                ActionDelay = TimeSpan.FromMinutes(MinutesDelay)
            };

            if (!string.IsNullOrWhiteSpace(_existingPeriodId))
            {
                noComputerPeriod.Id = _existingPeriodId;
            }

            return noComputerPeriod;
        }

        public TimeSpan EndTime
        {
            get => _endTime;
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;

                    NotifyPropertyChanged();
                }
            }
        }

        [Required]
        [Range(5, 30)]
        public int MinutesDelay
        {
            get => _minutesDelay;
            set
            {
                _minutesDelay = value;

                NotifyPropertyChanged();
            }
        }

        [DaySelectionRequired]
        public DaySelection DaysSelected
        {
            get => _daysSelected;
            set
            {
                _daysSelected = value;

                NotifyPropertyChanged();
            }
        }

        public IEnumerable<int> AvailableMinutesDelay { get; } = Enumerable.Range(3, 25);

        public ICommand CancelCommand => _cancelCommand;

        public ICommand SchedulePeriodCommand => _schedulePeriodCommand;

        private readonly string _existingPeriodId;
        private string _name;
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private int _minutesDelay;
        private DaySelection _daysSelected;
        private readonly CallbackCommand _cancelCommand;
        private readonly CallbackCommand _schedulePeriodCommand;

        public ScheduleNoComputerPeriodWindowViewModel()
        {
            _cancelCommand = new CallbackCommand(Cancel);
            _schedulePeriodCommand = new CallbackCommand(CanSchedulePeriod, SchedulePeriod);

            _minutesDelay = 5;
            _daysSelected = DaySelection.Daily;
        }

        public ScheduleNoComputerPeriodWindowViewModel(NoComputerPeriodViewModel existingNoComputerPeriod)
        {
            _cancelCommand = new CallbackCommand(Cancel);
            _schedulePeriodCommand = new CallbackCommand(CanSchedulePeriod, SchedulePeriod);

            _existingPeriodId = existingNoComputerPeriod.Id;
            _name = existingNoComputerPeriod.Name;
            _startTime = existingNoComputerPeriod.StartTime;
            _endTime = existingNoComputerPeriod.EndTime;
            _daysSelected = existingNoComputerPeriod.DaysActive;

            var minutesDelay = (int)existingNoComputerPeriod.ActionDelay.TotalMinutes;

            if (minutesDelay < AvailableMinutesDelay.Min())
            {
                minutesDelay = AvailableMinutesDelay.Min();
            }
            else if (minutesDelay > AvailableMinutesDelay.Max())
            {
                minutesDelay = AvailableMinutesDelay.Max();
            }

            _minutesDelay = minutesDelay;
        }

        protected override void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);

            _schedulePeriodCommand.Refresh();
        }

        private void Cancel(object parameter = default)
        {
            DialogResultSet.Invoke(false);
        }

        private bool CanSchedulePeriod(object parameter = default)
        {
            return Error == string.Empty;
        }

        private void SchedulePeriod(object parameter = default)
        {
            DialogResultSet.Invoke(true);
        }
    }
}
