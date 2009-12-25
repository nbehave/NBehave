using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class Feature
    {
        private readonly List<ScenarioWithSteps> _scenarios = new List<ScenarioWithSteps>();

        public Feature()
        {
            Title = string.Empty;
            Narrative = string.Empty;
        }

        public string Title { get; set; }
        public string Narrative { get; set; }
        public IEnumerable<ScenarioWithSteps> Scenarios { get { return _scenarios; } }

        public void AddScenario(ScenarioWithSteps scenario)
        {
            _scenarios.Add(scenario);
        }


        private Story _story;

        public Story AsStory()
        {
            if (_story == null)
            {
                _story = new Story(Title) { Narrative = Narrative };

            }
            return _story;
        }
    }
}