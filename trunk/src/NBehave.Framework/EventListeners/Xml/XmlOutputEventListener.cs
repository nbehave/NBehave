using System;
using System.Collections.Generic;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class XmlOutputEventListener : XmlOutputBase, IEventListener
    {
        private int _totalThemes = 0;
        private int _totalStories = 0;

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


        private ThemeXmlOutputWriter themeWriter;
        void IEventListener.ThemeStarted(string name)
        {
            themeWriter = new ThemeXmlOutputWriter(Writer, Actions);
            themeWriter.ThemeStarted(name);
            _totalThemes++;
        }

        void IEventListener.ThemeFinished()
        {
            if (storyWriter != null)
            {
                Actions.Enqueue(
                    () => Writer.WriteEndElement());
            }
            themeWriter.ThemeFinished();
            DoResults(_storyResults);
            storyWriter = null;
        }

        StoryXmlOutputWriter storyWriter;
        void IEventListener.StoryCreated(string story)
        {
            if (storyWriter == null)
            {
                Actions.Enqueue(
                    () => Writer.WriteStartElement("stories"));
            }
            storyWriter = new StoryXmlOutputWriter(Writer, Actions);
            storyWriter.StoryCreated(story);
            _totalStories++;
            themeWriter.TotalStories++;
        }

        void IEventListener.StoryMessageAdded(string message)
        {
            storyWriter.StoryMessageAdded(message);
        }

        private StoryResults _storyResults;
        void IEventListener.StoryResults(StoryResults results)
        {
            _storyResults = results;
            storyWriter.DoResults(results);
        }

        public override void DoResults(StoryResults results)
        {
            themeWriter.DoResults(_storyResults);
            base.DoResults(_storyResults);
        }
    }
}
