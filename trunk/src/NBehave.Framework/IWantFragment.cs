using System;
using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
    public class IWantFragment
    {
        private readonly Story _story;

        internal IWantFragment(string feature, Story story)
        {
            Debug.Assert(story != null);
            _story = story;
            _story.Narrative += Environment.NewLine + "I want " + feature;
            _story.OnMessageAdded(this, new EventArgs<MessageEventData>(new MessageEventData("Narrative", "I want " + feature)));
        }

        public SoThatFragment SoThat(string benefit)
        {
            return new SoThatFragment(benefit, _story);
        }

        public ScenarioBuilder WithScenario(string title)
        {
            return _story.WithScenario(title);
        }
    }
}