using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Views.Models.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PaulZero.RoutineEnforcer.Views.Controls
{
    /// <summary>
    /// Interaction logic for DayPicker.xaml
    /// </summary>
    public partial class DayPicker : UserControl, IDisposable
    {
        public static readonly DependencyProperty DaysSelectedProperty =
            DependencyProperty.Register(
                nameof(DaysSelected),
                typeof(DaySelection),
                typeof(DayPicker),
                new FrameworkPropertyMetadata(
                    DaySelection.Daily,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnDaySelectionChanged));

        private static void OnDaySelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            d.SetValue(e.Property, e.NewValue);
        }

        public DayPickerViewModel ViewModel
        {
            get => DataContext as DayPickerViewModel;
            set => DataContext = value;
        }

        public DaySelection DaysSelected
        {
            get { return (DaySelection)GetValue(DaysSelectedProperty); }
            set
            {
                SetValue(DaysSelectedProperty, value);
            }
        }

        public DayPicker()
        {
            InitializeComponent();

            ViewModel = new DayPickerViewModel();

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DaysSelected = ViewModel.Selection;
        }

        public void Dispose()
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }
}
