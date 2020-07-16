using System;
using System.Collections.Generic;
using System.Text;

namespace PaulZero.WindowsRoutine.Wpf.Models
{
    public class DaySelection
    {
        public bool Monday { get; set; }

        public bool Tuesday { get; set; }

        public bool Wednesday { get; set; }

        public bool Thursday { get; set; }

        public bool Friday { get; set; }

        public bool Saturday { get; set; }

        public bool Sunday { get; set; }

        public bool IsValidFor(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return Monday;
                case DayOfWeek.Tuesday:
                    return Tuesday;
                case DayOfWeek.Wednesday:
                    return Wednesday;
                case DayOfWeek.Thursday:
                    return Thursday;
                case DayOfWeek.Friday:
                    return Friday;
                case DayOfWeek.Saturday:
                    return Saturday;
                case DayOfWeek.Sunday:
                    return Sunday;

                default:
                    return false;
            }
        }

        public string[] GetEnabledDays()
        {
            var days = new List<string>();

            if (Monday)
            {
                days.Add("Monday");
            }

            if (Tuesday)
            {
                days.Add("Tuesday");
            }

            if (Wednesday)
            {
                days.Add("Wednesday");
            }

            if (Thursday)
            {
                days.Add("Thursday");
            }

            if (Friday)
            {
                days.Add("Friday");
            }

            if (Saturday)
            {
                days.Add("Saturday");
            }

            if (Sunday)
            {
                days.Add("Sunday");
            }

            return days.ToArray();
        }
    }
}
