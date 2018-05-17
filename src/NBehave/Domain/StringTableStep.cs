using System;
using System.Linq;
using System.Collections.Generic;
using NBehave.Extensions;
using NBehave.Internal;

namespace NBehave
{
    [Serializable]
    public class StringTableStep : StringStep
    {
        private readonly List<Example> tableSteps = new List<Example>();

        public StringTableStep(string token, string step, string source)
            : base(token, step, source)
        { }

        public StringTableStep(string token, string step, string source, int sourceLine)
            : base(token, step, source, sourceLine)
        { }

        public IEnumerable<Example> TableSteps
        {
            get
            {
                return tableSteps;
            }
        }

        public override IEnumerable<MethodParametersType> MatchableStepTypes
        {
            get
            {
                return new[] { MethodParametersType.TypedListStep, MethodParametersType.TypedStep, MethodParametersType.UntypedStep, MethodParametersType.UntypedListStep };
            }
        }

        public void AddTableStep(Example row)
        {
            tableSteps.Add(row);
        }

        public override StringStep BuildStep(Example values)
        {
            var template = MatchableStep;
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
            var clone = new StringTableStep(Token, template, Source, SourceLine);
            CloneTableSteps(clone);
            return clone;
        }

        private void CloneTableSteps(StringTableStep clone)
        {
            foreach (var tableStep in TableSteps)
            {
                var clonedValues = tableStep.ColumnValues.ToDictionary(pair => pair.Key, pair => pair.Value);
                var clonedNames = new ExampleColumns(tableStep.ColumnNames);
                var clonedRow = new Example(clonedNames, clonedValues);
                clone.AddTableStep(clonedRow);
            }
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine +
                "    " + tableSteps.First().ColumnNamesToString() + Environment.NewLine +
                string.Join(Environment.NewLine, tableSteps.Select(_ => "    " + _.ColumnValuesToString()).ToArray());
        }
    }
}