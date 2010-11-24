namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using global::System.Collections.Generic;
    using global::System.Linq;

    using NBehave.Narrator.Framework.EventListeners;

    using NUnit.Framework;

    [TestFixture]
    public class WhenRunningAScenarioWithScenarioOutlinesButNoActionSteps : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\ScenarioOutlines\ScenarioOutlinesWithPendingSteps.feature" });
        }

        protected override void Because()
        {
            this._results = this._config.Run();
        }

        [Test]
        public void ItShouldMarkAllResultsAsPending()
        {
            IEnumerable<Result> enumerable = this._results.ScenarioResults.First().ActionStepResults.Select(result => result.Result).ToList();
            CollectionAssert.AllItemsAreInstancesOfType(enumerable, typeof(Pending));
        }
    }
}