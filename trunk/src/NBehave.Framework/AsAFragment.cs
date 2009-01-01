using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
    public class AsAFragment
    {
        private readonly Story _story;

        internal AsAFragment(string role, Story story)
        {
            Debug.Assert(story != null);
            _story = story;
            _story.OnMessageAdded(this, new EventArgs<MessageEventData>(new MessageEventData("Narrative", "As a " + role)));
        }

        public IWantFragment IWant(string feature)
        {
            return new IWantFragment(feature, _story);
        }
    }
}