using System.Collections.Generic;
using System.Linq;

namespace NBehave.Domain
{
    public class ExampleTableFormatter
    {
        public string TableHeader(IEnumerable<Example> examples)
        {
            var columnSize = CalcColumnSize(examples);
            var columns = "|";
            foreach (var columnName in examples.First().ColumnNames)
                columns += FormatColumnValue(columnSize, columnName.Name, columnName.Name);
            return columns;
        }

        public string[] TableRows(IEnumerable<Example> examples)
        {
            var columnSize = CalcColumnSize(examples);
            var rows = new List<string>(examples.Count());
            foreach (var example in examples)
            {
                var row = "|";
                foreach (var columnName in example.ColumnNames)
                    row += FormatColumnValue(columnSize, columnName.Name, example.ColumnValues[columnName.Name.ToLower()]);
                rows.Add(row);
            }
            return rows.ToArray();
        }

        private Dictionary<string, int> CalcColumnSize(IEnumerable<Example> examples)
        {
            var columns = examples.First().ColumnNames.ToDictionary(example => example.Name, example => example.Name.Length);
            foreach (var example in examples)
            {
                foreach (var columnName in example.ColumnNames)
                {
                    var name = columnName.Name;
                    var valueLength = example.ColumnValues[name].Length;
                    if (columns[name] < valueLength)
                        columns[name] = valueLength;
                }
            }
            return columns;
        }

        private string FormatColumnValue(Dictionary<string, int> columnSize, string columnName, string columnValue)
        {
            return string.Format(" {0} |", columnValue.PadRight(columnSize[columnName]));
        }
    }
}