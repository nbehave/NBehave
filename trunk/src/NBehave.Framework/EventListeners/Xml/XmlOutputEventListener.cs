using System;
using System.Collections.Generic;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class XmlOutputEventListener : XmlOutputBase, IEventListener
    {
        private int _totalThemes = 0;
        private int _totalStories = 0;
        private ThemeXmlOutputWriter _themeWriter;
        StoryXmlOutputWriter _storyWriter;
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
                    string[] assemblyString = typeof(XmlOutputEventListener).AssemblyQualifiedName.Split(new char[] { ',' });
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
                    Writer.WriteEndElement();
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
        }

        void IEventListener.ThemeFinished()
        {
            if (_storyWriter != null)
            {
                Actions.Enqueue(
                    () => Writer.WriteEndElement());
            }
            _themeWriter.ThemeFinished();
            DoResults(_storyResults);
            _storyWriter = null;
        }

        void IEventListener.StoryCreated(string story)
        {
            if (_storyWriter == null)
            {
                Actions.Enqueue(
                    () => Writer.WriteStartElement("stories"));
            }
            _storyWriter = new StoryXmlOutputWriter(Writer, Actions);
            _storyWriter.StoryCreated(story);
            _totalStories++;
            _themeWriter.TotalStories++;
        }

        void IEventListener.StoryMessageAdded(string message)
        {
            _storyWriter.StoryMessageAdded(message);
        }

        void IEventListener.StoryResults(StoryResults results)
        {
            _storyResults = results;
            _storyWriter.DoResults(results);
        }

        public override void DoResults(StoryResults results)
        {
            _themeWriter.DoResults(_storyResults);
            base.DoResults(_storyResults);
        }
    }
}
