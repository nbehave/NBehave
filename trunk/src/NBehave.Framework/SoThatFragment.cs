using System;
using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
    public class SoThatFragment
    {
        private readonly Story _story;

        internal SoThatFragment(string benefit, Story story)
        {
            Debug.Assert(story != null);
            _story = story;
            _story.Narrative += Environment.NewLine + "So that " + benefit;
            _story.OnMessageAdded(this, new EventArgs<MessageEventData>(new MessageEventData("Narrative", "So that " + benefit)));
        }

        public ScenarioBuilder WithScenario(string title)
        {
            return _story.WithScenario(title);
        }
    }
}