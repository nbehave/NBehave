using System;
using System.Collections.Generic;
using System.Text;

namespace NBehave.Narrator.Framework
{
    public interface IEventListener
    {
        void StoryCreated(string story);
        void StoryMessageAdded(string message);
        void RunStarted();
        void RunFinished();
        void ThemeStarted(string name);
        void ThemeFinished();

        void StoryResults(StoryResults results);
    }
}