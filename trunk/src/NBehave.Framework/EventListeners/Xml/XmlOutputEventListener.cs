using System;
using System.Collections.Generic;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class XmlOutputEventListener : XmlOutputBase, IEventListener
    {
        private int _totalThemes;
        private int _totalStories;
        private int _totalScenarios;
        private ThemeXmlOutputWriter _themeWriter;
        private StoryXmlOutputWriter _storyWriter;
        private readonly List<ScenarioXmlOutputWriter> _scenarioWriters = new List<ScenarioXmlOutputWriter>();
        private ScenarioXmlOutputWriter _currentScenarioWriter;
        private StoryResults _storyResults;

        public XmlOutputEventListener(XmlWriter writer)
            : base(writer, new Queue<Action>())
        { }

        void IEventListener.RunStarted()
        {
            Actions.Enqueue(
                () =>
                {
                    Writer.WriteStartElement("results");
                    string[] assemblyString = typeof(XmlOutputEventListener).AssemblyQualifiedName.Split(new[] { ',' });
                    Writer.WriteAttributeString("name", assemblyString[1]);
                    Writer.WriteAttributeString("version", assemblyString[2]);
                    Writer.WriteAttributeString("date", DateTime.Today.ToShortDateString());
                    Writer.WriteAttributeString("time", DateTime.Now.ToShortTimeString());
                    Writer.WriteAttributeString("themes", _totalThemes.ToString());
                    Writer.WriteAttributeString("stories", _totalStories.ToString());
                    WriteScenarioResult();
                });
        }

        void IEventListener.RunFinished()
        {
            Actions.Enqueue(
                () =>
                {
                    Writer.WriteEndElement(); // </results>
                    Writer.Flush();
                });

            foreach (Action a in Actions)
                a.Invoke();
        }

        void IEventListener.ThemeStarted(string name)
        {
            _themeWriter = new ThemeXmlOutputWriter(Writer, Actions);
            _themeWriter.ThemeStarted(name);
            _totalThemes++;
            Actions.Enqueue(
                () => Writer.WriteStartElement("stories"));
        }

        void IEventListener.ThemeFinished()
        {
            if (_storyWriter != null)
            {
                Actions.Enqueue(
                    () => Writer.WriteEndElement()); // </stories>
            }
            _themeWriter.ThemeFinished();
            DoResults(_storyResults);
            _storyWriter = null;
        }

        void IEventListener.StoryCreated(string story)
        {
            _storyWriter = new StoryXmlOutputWriter(Writer, Actions);
            _storyWriter.StoryCreated(story);
            _totalStories++;
            _themeWriter.TotalStories++;
        }

        void IEventListener.StoryMessageAdded(string message)
        {
            _storyWriter.StoryMessageAdded(message);
        }

        void IEventListener.ScenarioCreated(string scenario)
        {
            if (_scenarioWriters.Count == 0)
            {
                Actions.Enqueue(() => Writer.WriteStartElement("scenarios"));
            }

            _currentScenarioWriter = new ScenarioXmlOutputWriter(Writer, Actions, _storyWriter.CurrentStoryTitle);
            _currentScenarioWriter.ScenarioCreated(scenario);
            _scenarioWriters.Add(_currentScenarioWriter);
            _totalScenarios++;
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
            _currentScenarioWriter.ScenarioMessageAdded(message);
        }

        void IEventListener.StoryResults(StoryResults results)
        {
            Actions.Enqueue(() => Writer.WriteEndElement()); // </scenarios>
            _storyResults = results;
            HandleScenarioResults(results);
            _storyWriter.DoResults(results);
        }

        private void HandleScenarioResults(StoryResults results)
        {
            foreach (var scenarioWriter in _scenarioWriters)
            {
                scenarioWriter.DoResults(results);
            }
            _scenarioWriters.Clear();
        }

        public override void DoResults(StoryResults results)
        {
            _themeWriter.DoResults(results);
            base.DoResults(results);
        }
    }
}
