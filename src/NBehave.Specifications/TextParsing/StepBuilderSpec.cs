using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Internal;
using NBehave.Internal.Gherkin;
using NBehave.TextParsing;
using NBehave.TextParsing.ModelBuilders;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Specifications.TextParsing
{
    [TestFixture]
    public class StepBuilderSpec
    {
        [Test]
        public void Should_add_steps_to_Scenario()
        {
            var gherkinEvents = MockRepository.GenerateStub<IGherkinParserEvents>();
            var stepBuilder = new StepBuilder(gherkinEvents);
            var feature = new Feature("title");
            var scenario = new Scenario("title", "source", feature);
            gherkinEvents.Raise(_ => _.FeatureEvent += null, this, new EventArgs<Feature>(feature));
            gherkinEvents.Raise(_ => _.ScenarioEvent += null, this, new EventArgs<Scenario>(scenario));
            gherkinEvents.Raise(_ => _.StepEvent += null, this, new EventArgs<StringStep>(new StringStep("Given", "step 1", "source")));
            gherkinEvents.Raise(_ => _.StepEvent += null, this, new EventArgs<StringStep>(new StringStep("Given", "step 2", "source")));
            gherkinEvents.Raise(_ => _.EofEvent += null, this, new EventArgs());

            Assert.That(scenario.Steps.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_add_table_steps_to_scenario()
        {
            var gherkinEvents = MockRepository.GenerateStub<IGherkinParserEvents>();
            var stepBuilder = new StepBuilder(gherkinEvents);
            var feature = new Feature("title");
            var scenario = new Scenario("title", "source", feature);
            gherkinEvents.Raise(_ => _.FeatureEvent += null, this, new EventArgs<Feature>(feature));
            gherkinEvents.Raise(_ => _.ScenarioEvent += null, this, new EventArgs<Scenario>(scenario));
            gherkinEvents.Raise(_ => _.StepEvent += null, this, new EventArgs<StringStep>(new StringStep("Given", "step 1", "source")));
            gherkinEvents.Raise(_ => _.StepEvent += null, this, new EventArgs<StringStep>(new StringStep("Given", "step 2", "source")));
            gherkinEvents.Raise(_ => _.TableEvent += null, this,
                                new EventArgs<IList<IList<Token>>>(new List<IList<Token>>
                                                                       {
                                                                           new List<Token>
                                                                               {
                                                                                   new Token("A", new LineInFile(1)),
                                                                                   new Token("B", new LineInFile(1))
                                                                               },
                                                                           new List<Token>
                                                                               {
                                                                                   new Token("1", new LineInFile(2)),
                                                                                   new Token("2", new LineInFile(2))
                                                                               },

                                                                       }));
            gherkinEvents.Raise(_ => _.EofEvent += null, this, new EventArgs());

            Assert.That(scenario.Steps.Count(), Is.EqualTo(2));
            Assert.That(scenario.Steps.ToList()[0], Is.TypeOf<StringStep>());
            Assert.That(scenario.Steps.ToList()[1], Is.TypeOf<StringTableStep>());
        }

        [Test]
        public void Should_add_docstring_to_previous_step()
        {
            var gherkinEvents = MockRepository.GenerateStub<IGherkinParserEvents>();
            var stepBuilder = new StepBuilder(gherkinEvents);
            var feature = new Feature("title");
            var scenario = new Scenario("title", "source", feature);
            gherkinEvents.Raise(_ => _.FeatureEvent += null, this, new EventArgs<Feature>(feature));
            gherkinEvents.Raise(_ => _.ScenarioEvent += null, this, new EventArgs<Scenario>(scenario));
            gherkinEvents.Raise(_ => _.StepEvent += null, this, new EventArgs<StringStep>(new StringStep("Given", "step 1", "source")));
            gherkinEvents.Raise(_ => _.StepEvent += null, this, new EventArgs<StringStep>(new StringStep("Given", "step 2", "source")));
            gherkinEvents.Raise(_ => _.DocStringEvent += null, this, new EventArgs<string>("docstring"));
            gherkinEvents.Raise(_ => _.EofEvent += null, this, new EventArgs());

            Assert.That(scenario.Steps.Count(), Is.EqualTo(2));
            var step = scenario.Steps.Last();
            Assert.IsTrue(step.HasDocString);
            Assert.AreEqual("docstring", step.DocString);
        }
    }
}