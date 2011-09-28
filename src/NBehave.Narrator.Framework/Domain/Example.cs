// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Example.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the Example type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class Example
    {
        public Example(ExampleColumns columnNames, Dictionary<string, string> columnValues)
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

        public override string ToString()
        {
            var s = ColumnNamesToString() + Environment.NewLine + ColumnValuesToString();
            return s;
        }

        public bool Equals(Example other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return ToString() == other.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals((Example)obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Example left, Example right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Example left, Example right)
        {
            return !Equals(left, right);
        }
    }
}