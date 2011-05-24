using System;
using System.Text;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class Row
    {
        public Dictionary<string, string> ColumnValues { get; private set; }
        public ExampleColumns ColumnNames { get; private set; }

        public Row(ExampleColumns columnNames, Dictionary<string, string> columnValues)
        {
            ColumnNames = columnNames;
            ColumnValues = columnValues;
        }

        protected Row()
        {}

        public string ColumnNamesToString()
        {
            var columnWidths = GetColumnWidths();
            return ValuesToString(s => s.PadRight(columnWidths[s]));
        }

        private Dictionary<string, int> GetColumnWidths()
        {
            var widths = new Dictionary<string, int>();
            foreach (var column in ColumnNames)
                widths.Add(column, ColumnValues[column].Length);
            return widths;
        }

        public string ColumnValuesToString()
        {
            return ValuesToString(s => ColumnValues[s]);
        }

        private string ValuesToString(Func<string, string> getValue)
        {
            var step = new StringBuilder();
            foreach (var columnName in ColumnNames)
                step.Append("|" + getValue(columnName));
            step.Append("|");
            return step.ToString();
        }
    }
}