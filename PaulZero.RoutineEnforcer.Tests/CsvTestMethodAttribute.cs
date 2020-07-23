using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace PaulZero.RoutineEnforcer.Tests
{
    internal class CsvTestMethodAttribute : TestMethodAttribute
    {
        public string CsvFilePath { get; }

        public Type CsvRowType { get; }

        private readonly PropertyInfo[] _properties;

        public CsvTestMethodAttribute(Type csvRowType, string csvFilePath)
        {
            CsvFilePath = csvFilePath;
            CsvRowType = csvRowType;

            _properties = CsvRowType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var testData = CreateTestData();
            var results = new List<TestResult>();

            foreach (var testRow in testData)
            {
                var result = testMethod.Invoke(new[] { testRow });

                result.DisplayName = $"{testMethod.TestMethodName} with values {CreateRowSummary(testRow)}";

                results.Add(result);
            }

            return results.ToArray();
        }

        private string CreateRowSummary(object row)
        {
            var propertySummaries = new List<string>();

            foreach (var prop in _properties)
            {
                propertySummaries.Add($"{prop.Name} = {prop.GetValue(row)}");
            }

            return string.Join(',', propertySummaries);
        }

        private object[] CreateTestData()
        {
            using var reader = new StreamReader(CsvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords(CsvRowType).ToArray();
        }
    }
}
