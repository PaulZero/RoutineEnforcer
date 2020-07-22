using System;

namespace PaulZero.RoutineEnforcer.Services.Notifications.NotificationViews
{
    internal abstract class AbstractNotificationView
    {
        public bool HasFailed { get; private set; }

        protected Guid NotificationId { get; }

        protected string Title { get; }

        protected string Message { get; }

        protected string ProgressStatusText { get; }

        protected string SkipButtonText { get; }

        protected AbstractNotificationView(Guid notificationId, string title, string message, string progressStatusText, string skipButtonText)
        {
            NotificationId = notificationId;
            Title = title;
            Message = message;
            ProgressStatusText = progressStatusText;
            SkipButtonText = skipButtonText;
        }

        public abstract void Show(NotificationUpdateData initialData);

        public abstract void Update(NotificationUpdateData updateData);

        public abstract void Hide();

        protected void MarkAsFailed()
        {
            HasFailed = true;
        }
    }
}
