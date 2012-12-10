// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleColumns.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ExampleColumns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    [Serializable]
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