using System.Runtime.InteropServices;

namespace PaulZero.WindowsRoutine.Wpf.Services.Actions
{
    internal class ActionService : IActionService
    {
        public void LockComputer()
        {
            LockWorkStation();
        }

        public void SleepComputer()
        {
            if (!SetSuspendState(false, true, true))
            {
                LockComputer();
            }
        }

        [DllImport("user32.dll")]
        internal static extern void LockWorkStation();

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
