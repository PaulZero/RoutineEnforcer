using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Services.Clock;
using PaulZero.RoutineEnforcer.Services.Clock.Interfaces;
using System;

namespace PaulZero.RoutineEnforcer.Services.Routine
{
    internal class NoComputerPeriodTimedCallback : AbstractTimedCallback
    {
        public NoComputerPeriod NoComputerPeriod { get; }

        public override bool IsPeriod => true;

        public NoComputerPeriodTimedCallback(NoComputerPeriod period, Action<ITimedCallback> callback)
            : base(callback)
        {
            NoComputerPeriod = period;
        }

        public override bool IsDue(DateTime currentDateTime)
        {
            return NoComputerPeriod.IsActiveAt(currentDateTime);
        }
    }
}
