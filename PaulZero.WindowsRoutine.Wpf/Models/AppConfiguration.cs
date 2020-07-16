using System.Collections.Generic;

namespace PaulZero.WindowsRoutine.Wpf.Models
{
    public class AppConfiguration
    {
        public List<ScheduledEvent> ScheduledEvents { get; set; }
            = new List<ScheduledEvent>();
    }
}
