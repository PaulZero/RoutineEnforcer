namespace PaulZero.RoutineEnforcer.Services.ComputerControl.Interfaces
{
    internal interface IComputerControlService
    {
        /// <summary>
        /// The computer will be put to sleep, if this does not work, then it will be locked instead.
        /// </summary>
        void SleepComputer();

        /// <summary>
        /// Windows will be locked forcing the user to log back in.
        /// </summary>
        void LockComputer();
    }
}
