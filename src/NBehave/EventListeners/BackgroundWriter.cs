using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBehave.EventListeners
{
    public class BackgroundWriter
    {
        private readonly TextWriter writer;

        public BackgroundWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(IEnumerable<BackgroundStepResult> backgroundSteps)
        {
            writer.WriteLine("Background: " + backgroundSteps.First().BackgroundTitle);
            foreach (var step in backgroundSteps)
                writer.WriteLine("  " + step.StringStep);
        }
    }
}