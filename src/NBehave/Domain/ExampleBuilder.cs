using System;
using System.Collections.Generic;

namespace NBehave
{
    public static class ExampleBuilder
    {
        public static Example BuildFromString(string str)
        {
            var cols = str.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var columnNames = BuildColumnNames(cols);
            var columnValues = BuildColumnValues(columnNames, cols);
            return new Example(columnNames, columnValues);
        }

        private static ExampleColumns BuildColumnNames(string[] cols)
        {
            var theMiddle = cols.Length / 2;
            var columnNames = new ExampleColumns();
            for (int i = 0; i < theMiddle; i++)
                columnNames.Add(new ExampleColumn(cols[i].Trim()));
            return columnNames;
        }

        private static Dictionary<string, string> BuildColumnValues(ExampleColumns columnNames, string[] cols)
        {
            var theMiddle = cols.Length / 2;
            var columnValues = new Dictionary<string, string>();
            for (int i = 0; i < theMiddle; i++)
            {
                string value = cols[1 + i + theMiddle].Trim();
                columnValues.Add(columnNames[i].Name, value);
            }
            return columnValues;
        }
    }
}