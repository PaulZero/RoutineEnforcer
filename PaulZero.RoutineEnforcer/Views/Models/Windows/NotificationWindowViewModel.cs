namespace PaulZero.RoutineEnforcer.Views.Models.Windows
{
    public class NotificationWindowViewModel : AbstractViewModel
    {
        public string Title { get; }

        public string Message { get; }

        public string ProgressStatusText { get; }

        public string SkipButtonText { get; }

        public int PercentageProgress
        {
            get => _percentageProgress;
            set
            {
                _percentageProgress = value;

                NotifyPropertyChanged();
            }
        }

        public string RemainingTime
        {
            get => _remainingTime;
            set
            {
                _remainingTime = value;

                NotifyPropertyChanged();
            }
        }

        private int _percentageProgress;
        private string _remainingTime;

        public NotificationWindowViewModel()
            : this("Screen lock incoming, lol", "You need to eat some cake", "Time until screen is locked...", "Lock screen already")
        {
        }

        public NotificationWindowViewModel(string title, string message, string progressStatusText, string skipButtonText)
        {
            Title = title;
            Message = message;
            ProgressStatusText = progressStatusText;
            SkipButtonText = skipButtonText;
        }
    }
}
