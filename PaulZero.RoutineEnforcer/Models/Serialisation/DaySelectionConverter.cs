using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaulZero.RoutineEnforcer.Models.Serialisation
{
    public class DaySelectionConverter : JsonConverter<DaySelection>
    {
        public override DaySelection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
            {
                return DaySelection.Empty;
            }

            if (value == "daily")
            {
                return DaySelection.Daily;
            }

            var days = value.Split(',');

            return new DaySelection
            {
                Monday = days.Contains(DaySelection.Names.Short.Monday),
                Tuesday = days.Contains(DaySelection.Names.Short.Tuesday),
                Wednesday = days.Contains(DaySelection.Names.Short.Wednesday),
                Thursday = days.Contains(DaySelection.Names.Short.Thursday),
                Friday = days.Contains(DaySelection.Names.Short.Friday),
                Saturday = days.Contains(DaySelection.Names.Short.Saturday),
                Sunday = days.Contains(DaySelection.Names.Short.Sunday)
            };
        }

        public override void Write(Utf8JsonWriter writer, DaySelection value, JsonSerializerOptions options)
        {
            if (value.Equals(DaySelection.Daily))
            {
                writer.WriteStringValue("daily");

                return;
            }

            var stringValue = string.Join(',', value.GetEnabledDays(true));

            writer.WriteStringValue(stringValue);
        }
    }
}
