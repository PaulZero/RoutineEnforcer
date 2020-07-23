using System;
using System.Collections.Generic;

namespace PaulZero.RoutineEnforcer.Models
{
    public struct DaySelection
    {
        public static class Names
        {
            public static class Short
            {
                public const string Monday = "mon";
                public const string Tuesday = "tue";
                public const string Wednesday = "wed";
                public const string Thursday = "thu";
                public const string Friday = "fri";
                public const string Saturday = "sat";
                public const string Sunday = "sun";
            }

            public static class Full
            {
                public const string Monday = "Monday";
                public const string Tuesday = "Tuesday";
                public const string Wednesday = "Wednesday";
                public const string Thursday = "Thursday";
                public const string Friday = "Friday";
                public const string Saturday = "Saturday";
                public const string Sunday = "Sunday";
            }
        }

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

        public string[] GetEnabledDays(bool useShortNames = false)
        {
            var days = new List<string>();

            if (Monday)
            {
                days.Add(useShortNames ? Names.Short.Monday : Names.Full.Monday);
            }

            if (Tuesday)
            {
                days.Add(useShortNames ? Names.Short.Tuesday : Names.Full.Tuesday);
            }

            if (Wednesday)
            {
                days.Add(useShortNames ? Names.Short.Wednesday : Names.Full.Wednesday);
            }

            if (Thursday)
            {
                days.Add(useShortNames ? Names.Short.Thursday : Names.Full.Thursday);
            }

            if (Friday)
            {
                days.Add(useShortNames ? Names.Short.Friday : Names.Full.Friday);
            }

            if (Saturday)
            {
                days.Add(useShortNames ? Names.Short.Saturday : Names.Full.Saturday);
            }

            if (Sunday)
            {
                days.Add(useShortNames ? Names.Short.Sunday : Names.Full.Sunday);
            }

            return days.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is DaySelection other)
            {
                return
                    other.Monday == Monday &&
                    other.Tuesday == Tuesday &&
                    other.Wednesday == Wednesday &&
                    other.Thursday == Thursday &&
                    other.Friday == Friday &&
                    other.Saturday == Saturday &&
                    other.Sunday == Sunday;
            }

            return false;
        }
    }
}
