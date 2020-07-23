namespace PaulZero.RoutineEnforcer.Tests.Data.Models.Serialisation
{
    public class DaySelectionConverterTestRow : ICsvTestDataRow
    {
        public string ExpectedValue { get; set; }

        public bool Monday { get; set; }

        public bool Tuesday { get; set; }

        public bool Wednesday { get; set; }

        public bool Thursday { get; set; }

        public bool Friday { get; set; }

        public bool Saturday { get; set; }

        public bool Sunday { get; set; }

        string ICsvTestDataRow.TestDescription
        {
            get
            {
                var values = new[]
                {
                    $"Expected Outcome: '{ExpectedValue}'",
                    $"Monday: {(Monday ? "yes" : "no")}",
                    $"Tuesday: {(Tuesday ? "yes" : "no")}",
                    $"Wednesday: {(Wednesday ? "yes" : "no")}",
                    $"Thursday: {(Thursday ? "yes" : "no")}",
                    $"Friday: {(Friday ? "yes" : "no")}",
                    $"Saturday: {(Saturday ? "yes" : "no")}",
                    $"Sunday: {(Sunday ? "yes" : "no")}",
                };

                return string.Join(", ", values);
            }
        }
    }
}
