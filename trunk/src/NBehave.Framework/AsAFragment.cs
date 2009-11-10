using System;
using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
    public class AsAFragment
    {
        private readonly Story _story;

        internal AsAFragment(string role, Story story)
        {
            Debug.Assert(story != null);
            _story = story;
            _story.Narrative = "As a " + role;
            _story.OnMessageAdded(this, new EventArgs<MessageEventData>(new MessageEventData("Narrative", _story.Narrative)));
        }

        public IWantFragment IWant(string feature)
        {
            return new IWantFragment(feature, _story);
        }
    }
}