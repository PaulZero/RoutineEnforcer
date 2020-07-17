using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaulZero.WindowsRoutine.Wpf.Models.View
{
    public class FallbackNotificationWindowViewModel : AbstractViewModel, IDisposable
    {
        public string Title { get; }

        public string Message { get; }

        public string ProgressStatusText { get; }

        public string SkipButtonText { get; }

        public int ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                _progressPercentage = value;

                NotifyPropertyChanged();
            }
        }

        public string TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                _timeRemaining = value;

                NotifyPropertyChanged();
            }
        }

        private readonly TimeSpan _delay;
        private int _progressPercentage;
        private string _timeRemaining;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public FallbackNotificationWindowViewModel()
            : this("Screen lock incoming, lol", "You need to eat some cake", "Time until screen is locked...", "Lock screen already", TimeSpan.FromMinutes(1))
        {
        }

        public FallbackNotificationWindowViewModel(string title, string message, string progressStatusText, string skipButtonText, TimeSpan delay)
        {
            Title = title;
            Message = message;
            ProgressStatusText = progressStatusText;
            SkipButtonText = skipButtonText;

            _delay = delay;

            ProgressPercentage = CalculateProgressPercentage(delay);
            TimeRemaining = FormatTimeRemaining(delay);
        }

        public void SkipTimer()
        {
            _tokenSource.Cancel();
        }

        public async Task WaitForCompletionAsync()
        {
            try
            {
                for (var i = 1; i <= _delay.TotalSeconds; i++)
                {
                    var remaining = _delay.Subtract(TimeSpan.FromSeconds(i));

                    ProgressPercentage = CalculateProgressPercentage(remaining);
                    TimeRemaining = FormatTimeRemaining(remaining);

                    await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);
                }
            }
            catch
            {
            }
        }

        private int CalculateProgressPercentage(TimeSpan remaining)
        {
            return (int)Math.Round((100 / _delay.TotalSeconds) * remaining.TotalSeconds);
        }

        private string FormatTimeRemaining(TimeSpan remaining)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
        }
    }
}
