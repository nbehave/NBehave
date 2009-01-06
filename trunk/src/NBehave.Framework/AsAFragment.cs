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

            _story.AddMessage("");
            _story.AddMessage("Narrative:");
            _story.AddMessage("\tAs a " + role);
        }

        public IWantFragment IWant(string feature)
        {
            return new IWantFragment(feature, _story);
        }
    }
}