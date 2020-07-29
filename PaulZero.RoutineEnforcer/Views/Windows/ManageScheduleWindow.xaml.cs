using PaulZero.RoutineEnforcer.Views.Models.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PaulZero.RoutineEnforcer.Views.Windows
{
    /// <summary>
    /// Interaction logic for ManageScheduleWindow.xaml
    /// </summary>
    public partial class ManageScheduleWindow : Window
    {
        public ManageScheduleWindowViewModel ViewModel
        {
            get => DataContext as ManageScheduleWindowViewModel;
            set => DataContext = value;
        }

        public ManageScheduleWindow() : this(new ManageScheduleWindowViewModel())
        {
        }

        public ManageScheduleWindow(ManageScheduleWindowViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }
    }
}
