using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
    public class SoThatFragment
    {
        private readonly Story _story;

        internal SoThatFragment(string benefit, Story story)
        {
            Debug.Assert(story != null);
            _story = story;
            _story.OnMessageAdded(this, new EventArgs<MessageEventData>(new MessageEventData("Narrative", "So that " + benefit)));
        }

        public ScenarioBuilder WithScenario(string title)
        {
            return _story.WithScenario(title);
        }
    }
}