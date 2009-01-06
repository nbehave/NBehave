using System;
using System.Collections.Generic;
using System.Xml;


namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class ThemeXmlOutputWriter : XmlOutputBase
    {
        private Timer currentThemeTimer;
        public int TotalStories { get; set; }

        public ThemeXmlOutputWriter(XmlWriter writer, Queue<Action> actions)
            : base(writer, actions)
        { }

        public void ThemeStarted(string name)
        {
            currentThemeTimer = new Timer();
            var themeTimer = currentThemeTimer; // so we have a reference to the correct theme when the code actually executes
            Actions.Enqueue(
                () =>
                {
                    WriteStartElement("theme", name, themeTimer);
                    Writer.WriteAttributeString("stories", TotalStories.ToString());
                    WriteScenarioResult();
                });
        }

        public void ThemeFinished()
        {
            currentThemeTimer.Stop();
            Actions.Enqueue(
               () => Writer.WriteEndElement()); // </theme>
        }
    }
}
