using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework.Extensions;
using NBehave.Narrator.Framework.Internal;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.Internal
{
    [TestFixture]
    public class FeatureRunnerSpec
    {
        private FeatureRunner featureRunner;
        private ActionCatalog actionCatalog;

        [SetUp]
        public void SetUp()
        {
            CreateActionCatalog();
            IStringStepRunner stringStepRunner = new StringStepRunner(actionCatalog);
            featureRunner = new FeatureRunner(stringStepRunner, MockRepository.GenerateStub<IRunContext>());
        }

        private void CreateActionCatalog()
        {
            actionCatalog = new ActionCatalog();
            Action<string> given = x => { };
            actionCatalog.Add(new ActionMethodInfo("name $x".AsRegex(), given, given.Method, "Given"));
            Action when = () => { };
            actionCatalog.Add(new ActionMethodInfo("greeted".AsRegex(), when, when.Method, "When"));
            Action<string> then = y => Assert.AreEqual("Nisse", y);
            actionCatalog.Add(new ActionMethodInfo("Hello $y".AsRegex(), then, then.Method, "Then"));
        }

        public class When_running_scenario_with_table_steps_that_fail : FeatureRunnerSpec
        {
            [Test]
            public void Should_Fail_scenario()
            {
                var feature = new Feature("title");
                var scenario = CreateScenarioWithTable(feature);
                feature.AddScenario(scenario);
                var result = featureRunner.Run(feature);
                Assert.IsNotNull(result);
                var se = result.ScenarioResults[0];
                Assert.IsTrue(se.HasFailedSteps());
            }

            private Scenario CreateScenarioWithTable(Feature feature)
            {
                var scenario = new Scenario("title", "", feature);
                var givenStep = new StringTableStep("Given name [x]", "");
                givenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("x") }), new Dictionary<string, string> { { "x", "Nisse" } }));
                givenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("x") }), new Dictionary<string, string> { { "x", "Kalle" } }));
                scenario.AddStep(givenStep);
                scenario.AddStep(new StringStep("When greeted", ""));
                var thenStep = new StringTableStep("Then Hello [y]", "");
                thenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("y"), }), new Dictionary<string, string> { { "y", "Nisse" } }));
                thenStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("y"), }), new Dictionary<string, string> { { "y", "Kålle" } }));
                scenario.AddStep(thenStep);
                return scenario;
            }
        }
    }
}