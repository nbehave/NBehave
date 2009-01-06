using System.IO;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class TextWriterEventListener : IEventListener
    {
        private readonly TextWriter writer;

        public TextWriterEventListener(TextWriter writer)
        {
            this.writer = writer;
        }

        void IEventListener.StoryCreated(string story)
        {
            writer.WriteLine("story created: {0}", story);
        }

        void IEventListener.StoryMessageAdded(string message)
        {
            writer.WriteLine("story message added: {0}", message);
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {
            writer.WriteLine("scenario created: {0}", scenarioTitle);            
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
            writer.WriteLine("scenario message added: {0}", message);
        }

        void IEventListener.RunStarted()
        {
            writer.WriteLine("run started");
        }

        void IEventListener.RunFinished()
        {
            writer.WriteLine("run finished");
        }

        void IEventListener.ThemeStarted(string name)
        {
            writer.WriteLine("theme started: {0}", name);
        }

        void IEventListener.ThemeFinished()
        {
            writer.WriteLine("theme finished");
        }

        void IEventListener.StoryResults(StoryResults results)
        {
        }

    }
}
