// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleColumns.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ExampleColumns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    public class ExampleColumns : List<ExampleColumn>
    {
        public ExampleColumns()
        {
        }

        public ExampleColumns(IEnumerable<ExampleColumn> columns)
            : base(columns)
        {
        }
    }
}