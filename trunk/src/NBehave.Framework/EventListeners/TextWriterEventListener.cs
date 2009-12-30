using System.IO;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class TextWriterEventListener : IEventListener
    {
        private readonly TextWriter _writer;

        public TextWriterEventListener(TextWriter writer)
        {
            _writer = writer;
        }

        void IEventListener.RunStarted()
        {
            _writer.WriteLine("run started");
        }

        void IEventListener.FeatureCreated(string feature)
        {
            _writer.WriteLine("Feature: {0}", feature);
        }

        void IEventListener.FeatureNarrative(string message)
        {
            _writer.WriteLine(message);
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {
            _writer.WriteLine("scenario created: {0}", scenarioTitle);
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
            _writer.WriteLine("scenario message added: {0}", message);
        }

        void IEventListener.RunFinished()
        {
            _writer.WriteLine("run finished");
        }

        void IEventListener.ThemeStarted(string name)
        {
            _writer.WriteLine("theme started: {0}", name);
        }

        void IEventListener.ThemeFinished()
        {
            _writer.WriteLine("theme finished");
        }

        void IEventListener.ScenarioResult(ScenarioResult result)
        {
            _writer.WriteLine("Scenario result: {0}", result.Result);
        }
    }
}