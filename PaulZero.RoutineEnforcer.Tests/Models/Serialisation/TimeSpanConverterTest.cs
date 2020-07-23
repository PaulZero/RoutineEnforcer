using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaulZero.RoutineEnforcer.Models.Serialisation;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PaulZero.RoutineEnforcer.Tests.Models.Serialisation
{
    [TestClass]
    public class TimeSpanConverterTest
    {
        [TestMethod]
        public void CanCorrectlySerialiseAndDeserialiseTimeSpanValues()
        {
            var test = new TestClass
            {
                Value = new TimeSpan(16, 0, 12)
            };

            var json = JsonSerializer.Serialize(test);

            Assert.IsTrue(json.Contains($"\"value\":\"16:00:12\"", StringComparison.OrdinalIgnoreCase));

            var deserialisedTest = JsonSerializer.Deserialize<TestClass>(json);

            Assert.AreEqual(test.Value, deserialisedTest.Value);
        }

        public class TestClass
        {
            [JsonConverter(typeof(TimeSpanConverter))]
            public TimeSpan Value { get; set; }
        }
    }
}
