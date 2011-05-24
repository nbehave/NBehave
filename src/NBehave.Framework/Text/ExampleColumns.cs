using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class ExampleColumns : List<string>
    {
        public ExampleColumns()
        { }

        public ExampleColumns(IEnumerable<string> columns)
            : base(columns)
        { }
    }
}