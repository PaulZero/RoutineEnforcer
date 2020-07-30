using Microsoft.Extensions.Logging;
using PaulZero.RoutineEnforcer.Services.ComputerControl.Interfaces;
using PaulZero.RoutineEnforcer.Services.Config.Interfaces;
using PaulZero.RoutineEnforcer.Views.Dialogs;
using System.Runtime.InteropServices;

namespace PaulZero.RoutineEnforcer.Services.ComputerControl
{
    internal class ComputerControlService : IComputerControlService
    {
        private bool EnableComputerControlActions => _configService.GetAppConfiguration().Options.EnableComputerControlActions;

        private readonly IConfigService _configService;
        private readonly ILogger _logger;

        public ComputerControlService(IConfigService configService, ILogger<IComputerControlService> logger)
        {
            _configService = configService;
            _logger = logger;
        }

        public void LockComputer()
        {
            if (!EnableComputerControlActions)
            {
                _logger.LogDebug("Computer lock was requested, but prevented due to configuration.");

                AlertDialog.ShowDialog("Computer Locked", "The computer would have been locked, but this has been disabled.");

                return;
            }

            _logger.LogDebug("Attempt to lock computer.");

            LockWorkStation();
        }

        public void SleepComputer()
        {
            if (!EnableComputerControlActions)
            {
                _logger.LogDebug("Computer sleep was requested, but prevented due to configuration.");

                AlertDialog.ShowDialog("Computer Sleeping", "The computer would have been put to sleep, but this has been disabled.");

                return;
            }

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
