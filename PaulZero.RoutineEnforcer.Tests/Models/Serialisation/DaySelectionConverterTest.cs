using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaulZero.RoutineEnforcer.Models;
using PaulZero.RoutineEnforcer.Models.Serialisation;
using PaulZero.RoutineEnforcer.Tests.Data.Models.Serialisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaulZero.RoutineEnforcer.Tests.Models.Serialisation
{
    [TestClass]
    public class DaySelectionConverterTest
    {
        public TestContext TestContext { get; set; }

        [CsvTestMethod(typeof(DaySelectionConverterTestRow), @"Data\Models\Serialisation\DaySelectionConverterTestData.csv")]
        [DeploymentItem(@"Data\Models\Serialisation\DaySelectionConverterTestData.csv")]
        public void CanCorrectlySerialiseAndDeserialiseDaySelectionValues(DaySelectionConverterTestRow row)
        {
            var test = new TestClass
            {
                Value = new DaySelection
                {
                    Monday = row.Monday,
                    Tuesday = row.Tuesday,
                    Wednesday = row.Wednesday,
                    Thursday = row.Thursday,
                    Friday = row.Friday,
                    Saturday = row.Saturday,
                    Sunday = row.Sunday
                }
            };

            var json = JsonSerializer.Serialize(test);

            Assert.IsTrue(json.Contains($"\"value\":\"{row.ExpectedValue}\"", StringComparison.OrdinalIgnoreCase));

            var deserialisedTest = JsonSerializer.Deserialize<TestClass>(json);

            Assert.AreEqual(test.Value, deserialisedTest.Value);
        }

        public class TestClass
        {
            [JsonConverter(typeof(DaySelectionConverter))]
            public DaySelection Value { get; set; }
        }

        public class TestDataSpec
        {
            public DaySelection DaySelection { get; }

            public string ExpectedOutput { get; }

            public TestDataSpec(string expectedOutput, bool monday = false, bool tuesday = false, bool wednesday = false, bool thursday = false, bool friday = false, bool saturday = false, bool sunday = false)
            {
                DaySelection = new DaySelection
                {
                    Monday = monday,
                    Tuesday = tuesday,
                    Wednesday = wednesday,
                    Thursday = thursday,
                    Friday = friday,
                    Saturday = saturday,
                    Sunday = sunday
                };

                ExpectedOutput = expectedOutput;
            }
        }
    }
}
