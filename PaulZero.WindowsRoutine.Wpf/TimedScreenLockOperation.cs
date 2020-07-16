using System;

namespace PaulZero.WindowsRoutine.Wpf
{
    public class TimedScreenLockOperation
    {
        private readonly TimeSpan _delay;
        private readonly string _description;

        public TimedScreenLockOperation(TimeSpan delay, string description)
        {
            _delay = delay;
            _description = description;
        }

        public void Start()
        {
        }



        private void DisplayWarningNotification()
        {
            // Construct the visuals of the toast (using Notifications library)

        }
    }
}
