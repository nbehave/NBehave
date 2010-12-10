// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Row.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the Row type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Row
    {
        public Row(ExampleColumns columnNames, Dictionary<string, string> columnValues)
        {
            ColumnNames = columnNames;
            ColumnValues = columnValues;
        }

        public Dictionary<string, string> ColumnValues { get; private set; }

        public ExampleColumns ColumnNames { get; private set; }

        public string ColumnNamesToString()
        {
            var columnWidths = GetColumnWidths();
            return ValuesToString(s => s.PadRight(columnWidths[s]));
        }

        public string ColumnValuesToString()
        {
            return ValuesToString(s => ColumnValues[s]);
        }

        private Dictionary<string, int> GetColumnWidths()
        {
            var widths = new Dictionary<string, int>();
            foreach (var column in ColumnNames)
            {
                widths.Add(column, ColumnValues[column].Length);
            }

            return widths;
        }

        private string ValuesToString(Func<string, string> getValue)
        {
            var step = new StringBuilder();
            foreach (var columnName in ColumnNames)
            {
                step.Append("|" + getValue(columnName));
            }

            step.Append("|");
            return step.ToString();
        }
    }
}