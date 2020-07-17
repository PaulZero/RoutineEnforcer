using System.ComponentModel;

namespace PaulZero.WindowsRoutine.Wpf.Models
{
    public enum EventActionType
    {
        [Description("Lock the screen")]
        LockScreen,
        [Description("Put the PC to sleep")]
        SleepComputer
    }
}
