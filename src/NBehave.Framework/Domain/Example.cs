// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Example.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the Example type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    public class Example : Row
    {
        public Example(ExampleColumns columnNames, Dictionary<string, string> columnValues)
            : base(columnNames, columnValues)
        {
        }
    }
}