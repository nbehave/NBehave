using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    [TestFixture]
    public class WhenRunningAScenarioWithScenarioOutlinesButNoActionSteps : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = ConfigurationNoAppDomain
                .New
                .SetAssemblies(new[] {Path.GetFileName(GetType().Assembly.Location)})
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] {@"System.Specs\Examples\ExamplesWithPendingSteps.feature"});
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void ItShouldMarkAllResultsAsPending()
        {
            IEnumerable<Result> enumerable = _results.First().ScenarioResults.First().StepResults.Select(result => result.Result).ToList();
            CollectionAssert.AllItemsAreInstancesOfType(enumerable, typeof (Pending));
        }
    }
}