using System;
using System.Collections.Generic;

namespace NBehave
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