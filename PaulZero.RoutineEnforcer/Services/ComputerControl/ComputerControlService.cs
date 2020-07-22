using Microsoft.Extensions.Logging;
using PaulZero.RoutineEnforcer.Services.ComputerControl.Interfaces;
using System.Runtime.InteropServices;

namespace PaulZero.RoutineEnforcer.Services.ComputerControl
{
    internal class ComputerControlService : IComputerControlService
    {
        private readonly ILogger _logger;

        public ComputerControlService(ILogger<IComputerControlService> logger)
        {
            _logger = logger;
        }

        public void LockComputer()
        {
            _logger.LogDebug("Attempt to lock computer.");

            LockWorkStation();
        }

        public void SleepComputer()
        {
            _logger.LogDebug("Attempting to put computer to sleep.");

            if (!SetSuspendState(false, true, true))
            {
                _logger.LogDebug("Failed to put computer to sleep, attempting to lock instead.");

                LockWorkStation();
            }
        }

        [DllImport("user32.dll")]
        internal static extern void LockWorkStation();

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
