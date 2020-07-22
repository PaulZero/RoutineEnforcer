using PaulZero.WindowsRoutine.Wpf.Services.Notifications.DisplayManagers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications
{
    class Notification : IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();

        protected string Title { get; }

        protected string Message { get; }

        protected string ProgressStatusText { get; }

        protected string SkipButtonText { get; }

        private readonly TimeSpan _delay;
        private AbstractDisplayManager _displayManager;
        private readonly ToastNotifier _toastNotifier;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public Notification(string title, string message, string progressStatusText, string skipButtonText, TimeSpan delay, ToastNotifier toastNotifier)
        {
            Title = title;
            Message = message;
            ProgressStatusText = progressStatusText;
            SkipButtonText = skipButtonText;

            _delay = delay;
            _toastNotifier = toastNotifier;
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
        }

        public void Skip()
        {
            _tokenSource.Cancel();
        }

        public async Task ShowAsync()
        {
            _displayManager = new ToastDisplayManager(_toastNotifier, Id, Title, Message, ProgressStatusText, SkipButtonText);

            _displayManager.Show(new DisplayManagerUpdateData(1, _delay, _delay));

            for (var i = 1; i <= _delay.TotalSeconds; i++)
            {
                var updateNumber = (uint)(i + 1);
                var updateData = new DisplayManagerUpdateData(updateNumber, _delay, _delay.Subtract(TimeSpan.FromSeconds(i)));

                if (_displayManager.HasFailed)
                {
                    _displayManager = new WindowDisplayManager(Id, Title, Message, ProgressStatusText, SkipButtonText);
                    _displayManager.Show(updateData);
                }
                else
                {
                    _displayManager.Update(updateData);
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);
                }
                catch
                {
                    break;
                }
            }

            _displayManager.Hide();
        }
    }
}
