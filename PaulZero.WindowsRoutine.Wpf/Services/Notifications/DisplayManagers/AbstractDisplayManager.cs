using System;

namespace PaulZero.WindowsRoutine.Wpf.Services.Notifications.DisplayManagers
{
    internal abstract class AbstractDisplayManager
    {
        public bool HasFailed { get; protected set; }

        protected Guid NotificationId { get; }

        protected string Title { get; }

        protected string Message { get; }

        protected string ProgressStatusText { get; }

        protected string SkipButtonText { get; }

        private readonly TimeSpan _delay;

        protected AbstractDisplayManager(Guid notificationId, string title, string message, string progressStatusText, string skipButtonText)
        {
            NotificationId = notificationId;
            Title = title;
            Message = message;
            ProgressStatusText = progressStatusText;
            SkipButtonText = skipButtonText;
        }

        public abstract void Show(DisplayManagerUpdateData initialData);

        public abstract void Update(DisplayManagerUpdateData updateData);

        public abstract void Hide();
    }
}
