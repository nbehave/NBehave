// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Row.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the Row type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace NBehave.Narrator.Framework
{

    [Serializable]
    public class Row
    {
        public Row(ExampleColumns columnNames, Dictionary<string, string> columnValues)
        {
            ColumnNames = columnNames;
            ColumnValues = new ColumnValues(columnValues);
        }

        public ColumnValues ColumnValues { get; private set; }

        public ExampleColumns ColumnNames { get; private set; }

        public string ColumnNamesToString()
        {
            return ValuesToString(_ => _.Name);
        }

        public string ColumnValuesToString()
        {
            return ValuesToString(s => ColumnValues[s.Name]);
        }

        private string ValuesToString(Func<ExampleColumn, string> getValue)
        {
            var step = new StringBuilder();
            foreach (var columnName in ColumnNames)
            {
                step.Append("| " + getValue(columnName));
            }

            step.Append(" |");
            return step.ToString();
        }
    }
}