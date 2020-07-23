using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaulZero.RoutineEnforcer.Models.Serialisation
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
            {
                return TimeSpan.Zero;
            }

            var parts = value.Split(':');

            if (parts.Length != 3)
            {
                return TimeSpan.Zero;
            }

            var intParts = parts.Select(p => int.Parse(p)).ToArray();

            return new TimeSpan(intParts[0], intParts[1], intParts[2]);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            var stringValue = $"{value.Hours:00}:{value.Minutes:00}:{value.Seconds:00}";

            writer.WriteStringValue(stringValue);
        }
    }
}
