﻿using System.Collections.Generic;

namespace PaulZero.RoutineEnforcer.Models
{
    public class AppConfiguration
    {
        public List<ScheduledEvent> ScheduledEvents { get; set; }
            = new List<ScheduledEvent>();

        public List<NoComputerPeriod> NoComputerPeriods { get; set; }
            = new List<NoComputerPeriod>();
    }
}
