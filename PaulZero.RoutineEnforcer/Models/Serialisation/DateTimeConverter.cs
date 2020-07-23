using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PaulZero.RoutineEnforcer.Models.Serialisation
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly Regex _dateTimeRegex = new Regex("^(?<day>[0-9]{1,2})/(?<month>[0-9]{1,2})/(?<year>[0-9]{4}) (?<hour>[0-9]{2}):(?<minute>[0-9]{2}):(?<second>[0-9]{2})$");

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (!_dateTimeRegex.IsMatch(value))
            {
                return DateTime.MinValue;
            }

            var match = _dateTimeRegex.Match(value);

            if (!match.Success)
            {
                return DateTime.MinValue;
            }

            var day = int.Parse(match.Groups["day"].Value);
            var month = int.Parse(match.Groups["month"].Value);
            var year = int.Parse(match.Groups["year"].Value);
            var hour = int.Parse(match.Groups["hour"].Value);
            var minute = int.Parse(match.Groups["minute"].Value);
            var second = int.Parse(match.Groups["second"].Value);

            return new DateTime(year, month, day, hour, minute, second);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var stringValue = $"{value.Day}/{value.Month}/{value.Year} {value.Hour}:{value.Minute}:{value.Second}";

            writer.WriteStringValue(stringValue);
        }
    }
}
