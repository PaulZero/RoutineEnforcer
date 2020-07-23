using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.Targets;
using PaulZero.RoutineEnforcer.Models.Serialisation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaulZero.RoutineEnforcer.Tests.Models.Serialisation
{
    [TestClass]
    public class DateTimeConverterTest
    {
        [TestMethod]
        public void CanCorrectlySerialiseAndDeserialiseDateTimeValues()
        {
            var test = new TestClass
            {
                Value = new DateTime(1974, 2, 11, 12, 15, 45)
            };

            var json = JsonSerializer.Serialize(test);

            Assert.IsTrue(json.Contains("\"value\":\"11/2/1974 12:15:45\"", StringComparison.OrdinalIgnoreCase));

            var deserialisedTest = JsonSerializer.Deserialize<TestClass>(json);

            Assert.AreEqual(test.Value, deserialisedTest.Value);
        }

        public class TestClass
        {
            [JsonConverter(typeof(DateTimeConverter))]
            public DateTime Value { get; set; }
        }
    }
}
