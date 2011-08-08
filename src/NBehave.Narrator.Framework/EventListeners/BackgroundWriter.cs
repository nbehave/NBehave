using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    public class BackgroundWriter
    {
        private readonly TextWriter _writer;

        public BackgroundWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void Write(IEnumerable<BackgroundStepResult> backgroundSteps)
        {
            _writer.WriteLine("Background: " + backgroundSteps.First().BackgroundTitle);
            foreach (var step in backgroundSteps)
                _writer.WriteLine(step.Message);
        }
    }
}