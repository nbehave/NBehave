using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NBehave.Domain;


namespace NBehave.Fluent.Framework.Extensions
{
    public static class FeatureExtensions
    {
        public static IAsAFragment AddStory(this Feature feature)
        {
            return new StoryBuilder.AsAFragment(feature);
        }

        public static IScenarioBuilderStartWithHelperObject AddScenario(this Feature feature)
        {
            return new ScenarioBuilder.StartFragment(feature);
        }

        public static IScenarioBuilderStartWithHelperObject AddScenario(this Feature feature, string scenarioTitle)
        {
            return new ScenarioBuilder.StartFragment(feature, scenarioTitle);
        }

        public static IEnumerable<IGrouping<Scenario, StringStep>> FindPendingSteps(this Feature feature)
        {
            return feature.Scenarios
                .Select(scenario => new
                                        {
                                            scenario,
                                            pendingSteps = scenario.Steps.Where(step => step.StepResult.Result is Pending)
                                        })
                .Where(scenarioStruct => scenarioStruct.pendingSteps.Count() > 0)
                .Select(scenarioStruct =>
                            {
                                IGrouping<Scenario, StringStep> g =
                                    new GroupingStructure<Scenario, StringStep>(
                                        scenarioStruct.scenario,
                                        scenarioStruct.pendingSteps);
                                return g;
                            });
        }
    }

    public class GroupingStructure<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey _key;
        private readonly IEnumerable<TElement> _elements;

        public GroupingStructure(TKey key, IEnumerable<TElement> elements)
        {
            _key = key;
            _elements = elements;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        public TKey Key
        {
            get { return _key; }
        }
    }
}