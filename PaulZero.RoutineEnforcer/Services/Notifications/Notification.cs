using PaulZero.RoutineEnforcer.Services.Notifications.NotificationViews;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace PaulZero.RoutineEnforcer.Services.Notifications
{
    internal class Notification : IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string FailureMessage { get; private set; }

        protected string Title { get; }

        protected string Message { get; }

        protected string ProgressStatusText { get; }

        protected string SkipButtonText { get; }

        private NotificationResult _result = NotificationResult.Failed;
        private AbstractNotificationView _displayManager;

        private readonly TimeSpan _delay;
        private readonly bool _forceWindowNotifications;
        private readonly ToastNotifier _toastNotifier;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Notification(string title, string message, string progressStatusText, string skipButtonText, TimeSpan delay, ToastNotifier toastNotifier, bool forceWindowNotifications = false)
        {
            Title = title;
            Message = message;
            ProgressStatusText = progressStatusText;
            SkipButtonText = skipButtonText;

            _delay = delay;
            _forceWindowNotifications = forceWindowNotifications;
            _toastNotifier = toastNotifier;
        }

        public void Abort()
        {
            _result = NotificationResult.Aborted;

            _displayManager.Hide();

            _cancellationTokenSource.Cancel();
        }

        public void Skip()
        {
            _result = NotificationResult.Skipped;

            _cancellationTokenSource.Cancel();
        }

        public async Task<NotificationResult> ShowAsync()
        {
            try
            {
                if (_forceWindowNotifications)
                {
                    _displayManager = new WindowNotificationView(Id, Title, Message, ProgressStatusText, SkipButtonText);
                }
                else
                {
                    _displayManager = new ToastNotificationView(_toastNotifier, Id, Title, Message, ProgressStatusText, SkipButtonText);
                }

                _displayManager.Show(new NotificationUpdateData(1, _delay, _delay));

                for (var i = 1; i <= _delay.TotalSeconds; i++)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var updateNumber = (uint)(i + 1);
                    var updateData = new NotificationUpdateData(updateNumber, _delay, _delay.Subtract(TimeSpan.FromSeconds(i)));

                    if (_displayManager.HasFailed)
                    {
                        if (_displayManager is WindowNotificationView)
                        {
                            // Already failed using windows, oh dear...

                            FailureMessage = "Display manager failed, could not fall back to windows as it was already using them.";

                            return NotificationResult.Failed;
                        }

                        _displayManager = new WindowNotificationView(Id, Title, Message, ProgressStatusText, SkipButtonText);
                        _displayManager.Show(updateData);
                    }
                    else
                    {
                        _displayManager.Update(updateData);
                    }
                    
                    await Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token);
                }

                _displayManager.Hide();

                return NotificationResult.Successful;
            }
            catch (OperationCanceledException)
            {
                return _result;
            }
            catch (Exception exception)
            {
                FailureMessage = exception.Message;

                return NotificationResult.Failed;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}
