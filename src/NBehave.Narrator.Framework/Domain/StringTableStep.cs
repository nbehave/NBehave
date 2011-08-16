// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTableStep.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringTableStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    public class StringTableStep : StringStep
    {
        private readonly List<Row> _tableSteps = new List<Row>();

        public StringTableStep(string step, string source)
            : base(step, source)
        { }

        public IEnumerable<Row> TableSteps
        {
            get
            {
                return _tableSteps;
            }
        }

        public void AddTableStep(Row row)
        {
            _tableSteps.Add(row);
        }

        public override StringStep BuildStep(Row values)
        {
            var template = Step;
            foreach (var columnName in values.ColumnNames)
            {
                var columnValue = values.ColumnValues[columnName.Name].TrimWhiteSpaceChars();
                var replace = BuildColumnValueReplaceRegex(columnName);
                template = replace.Replace(template, columnValue);

                foreach (var row in TableSteps)
                {
                    var newValues = row.ColumnValues.ToDictionary(pair => pair.Key, pair => replace.Replace(pair.Value, columnValue));
                    row.ColumnValues.Clear();
                    foreach (var pair in newValues)
                        row.ColumnValues.Add(pair.Key, pair.Value);
                }
            }
            var clone = new StringTableStep(template, Source);
            CloneTableSteps(clone);
            return clone;
        }

        private void CloneTableSteps(StringTableStep clone)
        {
            foreach (var tableStep in TableSteps)
            {
                var clonedValues = tableStep.ColumnValues.ToDictionary(pair => pair.Key, pair => pair.Value);
                var clonedNames = new ExampleColumns(tableStep.ColumnNames);
                var clonedRow = new Row(clonedNames, clonedValues);
                clone.AddTableStep(clonedRow);
            }
        }
    }
}