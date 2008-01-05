using System.Diagnostics;

namespace NBehave.Narrator.Framework
{
    public class IWantFragment
    {
        private readonly Story _story;

        internal IWantFragment(string feature, Story story)
        {
            Debug.Assert(story != null);

            _story = story;

            _story.AddMessage("\tI want " + feature);
        }

        public SoThatFragment SoThat(string benefit)
        {
            return new SoThatFragment(benefit, _story);
        }

        public Scenario WithScenario(string title)
        {
            return _story.WithScenario(title);
        }
    }
}