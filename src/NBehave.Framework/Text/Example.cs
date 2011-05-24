using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class Example : Row
    {
        protected Example()
        {}

        public Example(ExampleColumns columnNames, Dictionary<string, string> columnValues)
            : base(columnNames, columnValues)
        { }
    }
}