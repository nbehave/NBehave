using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications.System.Specs.Examples
{
    [TestFixture]
    public class WhenRunningAScenarioWithScenarioOutlinesButNoActionSteps : SystemTestContext
    {
        private FeatureResults _results;

        protected override void EstablishContext()
        {
           Configure_With(Path.Combine("System.Specs", "Examples", "ExamplesWithPendingSteps.feature"));
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
