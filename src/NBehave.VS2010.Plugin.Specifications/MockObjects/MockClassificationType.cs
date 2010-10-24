using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.Specs
{
    public class MockClassificationType : IClassificationType
    {
        public bool IsOfType(string type)
        {
            return Classification == type;
        }

        public string Classification { get; set; }

        public IEnumerable<IClassificationType> BaseTypes { get; set; }
    }
}