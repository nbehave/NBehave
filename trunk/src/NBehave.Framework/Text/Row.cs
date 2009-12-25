using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class Row
    {
        public Dictionary<string, string> ColumnValues { get; private set; }
        public ExampleColumns ColumnNames { get; private set; }

        public Row(ExampleColumns columnNames, Dictionary<string, string> columnValues)
        {
            ColumnNames = columnNames;
            ColumnValues = columnValues;
        }
    }
}