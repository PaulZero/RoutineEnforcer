using System;
using System.Collections.Generic;

namespace PaulZero.RoutineEnforcer.Models
{
    public struct DaySelection
    {
        public static DaySelection Daily =>
            new DaySelection
            {
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = true,
                Friday = true,
                Saturday = true,
                Sunday = true
            };

        public static DaySelection Empty =>
            new DaySelection
            {
                Monday = false,
                Tuesday = false,
                Wednesday = false,
                Thursday = false,
                Friday = false,
                Saturday = false,
                Sunday = false
            };

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

        public DaySelection WithMonday(bool value)
        {
            Monday = value;

            return this;
        }

        public DaySelection WithTuesday(bool value)
        {
            Tuesday = value;

            return this;
        }

        public DaySelection WithWednesday(bool value)
        {
            Wednesday = value;

            return this;
        }

        public DaySelection WithThursday(bool value)
        {
            Thursday = value;

            return this;
        }

        public DaySelection WithFriday(bool value)
        {
            Friday = value;

            return this;
        }
        public DaySelection WithSaturday(bool value)
        {
            Saturday = value;

            return this;
        }

        public DaySelection WithSunday(bool value)
        {
            Sunday = value;

            return this;
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
