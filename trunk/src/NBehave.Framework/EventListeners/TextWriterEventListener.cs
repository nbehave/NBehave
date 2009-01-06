using System.IO;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class TextWriterEventListener : IEventListener
    {
        private readonly TextWriter writer;

        public TextWriterEventListener(TextWriter writer)
        {
            this.writer = writer;
        }

        #region IEventListener Members

        public void StoryCreated(string story)
        {
            writer.WriteLine("story created");
        }

        public void StoryMessageAdded(string message)
        {
            writer.WriteLine("story message added: {0}", message);
        }

        public void RunStarted()
        {
            writer.WriteLine("run started");
        }

        public void RunFinished()
        {
            writer.WriteLine("run finished");
        }

        public void ThemeStarted(string name)
        {
            writer.WriteLine("theme started: {0}", name);
        }

        public void ThemeFinished()
        {
            writer.WriteLine("theme finished");
        }

        public void StoryResults(StoryResults results)
        {
        }

        #endregion
    }
}
