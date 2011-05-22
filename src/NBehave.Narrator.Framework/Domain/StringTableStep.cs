// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTableStep.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StringTableStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    public class StringTableStep : StringStep
    {

        private readonly List<Row> _tableSteps = new List<Row>();

        public StringTableStep(string step, string fromFile)
            : base(step, fromFile)
        {
        }

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
    }
}