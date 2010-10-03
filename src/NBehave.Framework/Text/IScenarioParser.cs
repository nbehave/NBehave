using System.Collections.Generic;
using System.IO;

namespace NBehave.Narrator.Framework
{
    public interface IScenarioParser
    {
        IEnumerable<Feature> Parse(Stream stream);
    }
}