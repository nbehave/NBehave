using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.Processors
{
    [TestFixture]
    public class FeatureRunnerSpec
    {
        private FeatureRunner featureRunner;
        private ITinyMessengerHub hub;
        private ActionCatalog actionCatalog;

        [SetUp]
        public void SetUp()
        {
            CreateActionCatalog();
            hub = MockRepository.GenerateStub<ITinyMessengerHub>();
            IStringStepRunner stringStepRunner = new StringStepRunner(actionCatalog, hub);
            featureRunner = new FeatureRunner(hub, stringStepRunner);
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
                featureRunner.Run(feature);
                var args = hub.GetArgumentsForCallsMadeOn(_ => _.Publish<ScenarioResultEvent>(null));
                Assert.IsNotNull(args);
                var se = (ScenarioResultEvent)(args[0][0]);
                Assert.IsTrue(se.Content.HasFailedSteps());
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