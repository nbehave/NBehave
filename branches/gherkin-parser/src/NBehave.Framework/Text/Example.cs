using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class Example : Row
    {
        public Example(ExampleColumns columnNames, Dictionary<string, string> columnValues)
            : base(columnNames, columnValues)
        { }
    }
}