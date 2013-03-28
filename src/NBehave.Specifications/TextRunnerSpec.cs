using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Configuration;
using NBehave.EventListeners;
using NBehave.EventListeners.Xml;
using NBehave.Extensions;
using NBehave.Internal;
using NBehave.Narrator.Framework.Tiny;
using NBehave.Specifications.Features;
using NUnit.Framework;

namespace NBehave.Specifications
{
    [TestFixture]
    public class TextRunnerSpec
    {
        private IRunner CreateRunnerWithBasicConfiguration()
        {
            var config = CreateBasicConfiguration();
            return config.Build();
        }

        private NBehaveConfiguration CreateBasicConfiguration()
        {
            var writer = new StreamWriter(new MemoryStream());
            var listener = new TextWriterEventListener(writer);

            var config = ConfigurationNoAppDomain
                .New
                .SetAssemblies(new[] { "TestLib.dll" })
                .SetEventListener(listener)
                .SetScenarioFiles(new[] { TestFeatures.FeatureWithManyScenarios });
            return config;
        }

        [TestFixture]
        public class WhenInitialisingBeforeRunningPlainTextScenarios : TextRunnerSpec
        {
            [Test]
            public void ShouldFindGivenActionStepInAssembly()
            {
                TinyIoCContainer tinyIoCContainer = TinyIoCContainer.Current;

                var runner = CreateRunnerWithBasicConfiguration();
                runner.Run();

                var actionCatalog = tinyIoCContainer.Resolve<ActionCatalog>();

                Assert.That(actionCatalog.ActionExists("Given my name is Morgan".AsStringStep("")), Is.True);
            }

            [Test]
            public void ShouldFindWhenActionStepInAssembly()
            {
                var runner = CreateRunnerWithBasicConfiguration();
                runner.Run();

                var actionCatalog = TinyIoCContainer.Current.Resolve<ActionCatalog>();

                Assert.That(actionCatalog.ActionExists("Given I'm greeted".AsStringStep("")), Is.True);
            }

            [Test]
            public void ShouldFindThenActionStepInAssembly()
            {
                var runner = CreateRunnerWithBasicConfiguration();
                runner.Run();

                var actionCatalog = TinyIoCContainer.Current.Resolve<ActionCatalog>();

                Assert.That(actionCatalog.ActionExists("Given I should be greeted with “Hello, Morgan!”".AsStringStep("")), Is.True);
            }
        }

        [TestFixture]
        public class WhenRunningPlainTextScenarios : TextRunnerSpec
        {
            private FeatureResults _results;

            [Test]
            public void ShouldGetCorrectErrormessageFromFailedScenario()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.FeatureWithFailingStep })
                    .Build()
                    .Run();

                Assert.That(_results.NumberOfFailingScenarios, Is.EqualTo(1));
                Assert.That(_results[0].ScenarioResults[0].Message.StartsWith("Should.Core.Exceptions"), Is.True);
            }

            [Test]
            public void ShouldMarkFailingStepAsFailedInOutput()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                CreateBasicConfiguration().SetEventListener(listener).SetScenarioFiles(new[] { TestFeatures.FeatureWithFailingStep })
                    .Build()
                    .Run();

                StringAssert.Contains("Then I should be greeted with “Hello, Scott!” - FAILED", writer.ToString());
            }

            [Test]
            public void ShouldExecuteMoreThanOneScenarioInTextFile()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.FeatureWithManyScenarios })
                    .Build()
                    .Run();

                Assert.That(_results.NumberOfScenariosFound, Is.EqualTo(2));
                Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(2));
            }

            [Test]
            public void ShouldRunScenarioInTextFileWithScenarioTitle()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.ScenariosWithoutFeature })
                    .Build()
                    .Run();

                Assert.That(_results[0].ScenarioResults[0].ScenarioTitle, Is.EqualTo("greeting Morgan"));
                Assert.That(_results[0].ScenarioResults[0].Result, Is.TypeOf(typeof(Passed)));
            }

            [Test]
            public void ShouldRunTextScenarioWhithNewlinesInGiven()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.FeatureWithNewLineInGivenClause })
                    .Build()
                    .Run();

                Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void ShouldSetScenarioPendingIfActionGivenInTokenStringDoesntExist()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.ScenarioWithNoActionSteps })
                    .Build()
                    .Run();

                Assert.That(_results.NumberOfPendingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void ShouldListAllPendingActionSteps()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.ScenarioWithNoActionSteps }).Build().Run();

                StringAssert.Contains("No matching Action found for \"Given something that has no ActionStep\"", _results[0].ScenarioResults[0].Message);
                StringAssert.Contains("No matching Action found for \"And something else that has no ActionStep\"", _results[0].ScenarioResults[0].Message);
            }

            [Test]
            public void ShouldUseWildcardAndRunAllScenariosInAllMatchingTextFiles()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { @"Features\\Feature*.feature" }).Build().Run();

                Assert.That(_results.Count, Is.EqualTo(12));
                Assert.That(_results.NumberOfScenariosFound, Is.EqualTo(17));
                Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(8));
            }

            [Test]
            public void Should_not_crash_when_steps_are_written_in_lower_case()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.FeatureWithLowerCaseSteps }).Build().Run();

                Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void Should_not_crash_when_feature_file_ends_with_comment()
            {
                _results = CreateBasicConfiguration().SetScenarioFiles(new[] { TestFeatures.FeatureWithCommentOnLastRow }).Build().Run();

                Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class WhenRunningWithXmlListener : TextRunnerSpec
        {
            private XmlDocument _xmlOut;

            [SetUp]
            public void SetUp()
            {
                var writer = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);
                var listener = new XmlOutputEventListener(writer);

                CreateBasicConfiguration()
                    .SetEventListener(listener)
                    .SetScenarioFiles(new[] { TestFeatures.FeatureWithFailingStep })
                    .Build()
                    .Run();

                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            [Test]
            public void ShouldFindOneFailedStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//step[@outcome='failed']");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Test]
            public void ShouldFindTwoPassedStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//step[@outcome='passed']");
                Assert.That(storyNodes.Count, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class WhenRunningPlainTextScenariosWithXmlListener : TextRunnerSpec
        {
            private const string StoryTitle = "Greeting system";

            private XmlDocument _xmlOut;

            [SetUp]
            public void SetUp()
            {
                var writer = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);
                var listener = new XmlOutputEventListener(writer);
                CreateBasicConfiguration()
                    .SetEventListener(listener)
                    .SetScenarioFiles(new[] { TestFeatures.FeatureWithManyScenarios })
                    .DontIsolateInAppDomain()
                    .Build()
                    .Run();

                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            [Test]
            public void ShouldFindOneFeature()
            {
                var storyNodes = _xmlOut.SelectNodes("//feature");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Test]
            public void ShouldSetTitleOfFeature()
            {
                var storyNodes = _xmlOut.SelectSingleNode("//feature").Attributes["name"];

                Assert.That(storyNodes.Value, Is.EqualTo(StoryTitle));
            }

            [Test]
            public void ShouldRunTwoScenarios()
            {
                var scenarioNodes = _xmlOut.SelectNodes("//scenario");

                Assert.That(scenarioNodes.Count, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class WhenRunningPlainTextScenariosWithFeature : TextRunnerSpec
        {
            private FeatureResults _results;
            private StringWriter _messages;

            [SetUp]
            public void SetUp()
            {
                _messages = new StringWriter();
                var listener = new TextWriterEventListener(_messages);

                _results = CreateBasicConfiguration()
                    .SetEventListener(listener)
                    .SetScenarioFiles(new[] { TestFeatures.FeatureNamedStory })
                    .Build()
                    .Run();
            }

            [Test]
            public void ShouldSetFeatureTitleOnResult()
            {
                Assert.That(_results[0].ScenarioResults[0].FeatureTitle, Is.EqualTo("Greeting system"));
            }

            [Test]
            public void ShouldSetNarrativeOnResult()
            {
                var messages = _messages.ToString();
                StringAssert.Contains("As a project member", messages);
                StringAssert.Contains("I want", messages);
                StringAssert.Contains("So that", messages);
            }

            [Test]
            public void ShouldSetScenarioTitleOnResult()
            {
                Assert.That(_results[0].ScenarioResults[0].ScenarioTitle, Is.EqualTo("Greeting someone"));
            }
        }

        [TestFixture, ActionSteps]
        public class WhenRunningPlainTextScenarioInSwedish : TextRunnerSpec
        {
            private readonly Stack<int> _numbers = new Stack<int>();
            private int _calcResult;
            private static bool _givenWasCalled;
            private static bool _whenWasCalled;
            private static bool _thenWasCalled;
            private FeatureResults _featureResults;

            [Given(@"att jag knappat in $number")]
            public void GivenNumber(int number)
            {
                _numbers.Push(number);
                _givenWasCalled = true;
            }

            [When("jag summerar")]
            public void Sum()
            {
                _calcResult = _numbers.Pop() + _numbers.Pop();
                _whenWasCalled = true;
            }

            [Then("ska resultatet vara $sum")]
            public void Result(int sum)
            {
                Assert.AreEqual(sum, _calcResult);
                _thenWasCalled = true;
            }

            [TestFixtureSetUp]
            public void SetUp()
            {
                _featureResults = CreateBasicConfiguration()
                    .SetAssemblies(new[] { Path.GetFileName(GetType().Assembly.Location) })
                    .SetScenarioFiles(new[] { TestFeatures.FeatureInSwedish })
                    .Build()
                    .Run();
            }

            [Test]
            public void ShouldRunTextScenarioInStream()
            {
                Assert.That(_featureResults.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void GivenShouldBeCalled()
            {
                Assert.That(_givenWasCalled, Is.True);
            }

            [Test]
            public void WhenShouldBeCalled()
            {
                Assert.That(_whenWasCalled, Is.True);
            }

            [Test]
            public void ThenShouldBeCalled()
            {
                Assert.That(_thenWasCalled, Is.True);
            }
        }

        [TestFixture]
        public class When_running_scenario_that_fails : TextRunnerSpec
        {

            private class Listener : EventListener
            {
                private readonly Action featureFinished;
                private readonly Action scenarioFinished;

                public Listener(Action featureFinished, Action scenarioFinished)
                {
                    this.featureFinished = featureFinished;
                    this.scenarioFinished = scenarioFinished;
                }

                public override void FeatureFinished(FeatureResult result)
                {
                    featureFinished();
                }

                public override void ScenarioFinished(ScenarioResult result)
                {
                    scenarioFinished();
                }
            }

            [Test]
            public void Should_get_FeatureFinishedEvent()
            {
                bool featureFinishedEvent = false;
                var e = new Listener(() => featureFinishedEvent = true, () => { });
                var runner = CreateBasicConfiguration()
                    .SetScenarioFiles(new[] { TestFeatures.FeatureWithFailingStep })
                    .SetEventListener(e)
                    .Build();
                runner.Run();
                Assert.That(featureFinishedEvent, Is.True);
            }

            [Test]
            public void Should_get_ScenarioFinishedEvent()
            {
                bool scenarioFinishedEvent = false;
                var e = new Listener(() => { }, () => scenarioFinishedEvent = true);
                var runner = CreateBasicConfiguration()
                    .SetScenarioFiles(new[] { TestFeatures.FeatureWithFailingStep })
                    .SetEventListener(e)
                    .Build();
                runner.Run();

                Assert.That(scenarioFinishedEvent, Is.True);
            }
        }

        [TestFixture, Hooks.Hooks, ActionSteps]
        public class HooksSpec : TextRunnerSpec
        {
            private static int _timesBeforeScenarioWasCalled;
            private static int _timesBeforeStepWasCalled;
            private static int _timesAfterStepWasCalled;
            private static int _timesAfterScenarioWasCalled;
            private static int _beforeRun;
            private static int _afterRun;
            private static int _beforeFeature;
            private static int _afterFeature;
            private static bool _backgroundStepWasCalledButNotBeforeScenario;
            private static bool _backgroundStepWasCalledButNotBeforeStep;

            [Hooks.BeforeRun]
            public void OnBeforeRun()
            {
                _beforeRun++;
            }
            [Hooks.AfterRun]
            public void OnAfterRun()
            {
                _afterRun++;
            }
            [Hooks.BeforeFeature]
            public void OnBeforeFeature()
            {
                _beforeFeature++;
            }
            [Hooks.AfterFeature]
            public void OnAfterFeature()
            {
                _afterFeature++;
            }
            [Hooks.BeforeScenario]
            public void OnBeforeScenario()
            {
                _timesBeforeScenarioWasCalled++;
            }
            [Hooks.BeforeStep]
            public void OnBeforeStep()
            {
                _timesBeforeStepWasCalled++;
            }
            [Hooks.AfterStep]
            public void OnAfterStep()
            {
                _timesAfterStepWasCalled++;
            }
            [Hooks.AfterScenario]
            public void OnAfterScenario()
            {
                _timesAfterScenarioWasCalled++;
            }

            [TestFixtureSetUp]
            public void Setup()
            {
                _timesBeforeScenarioWasCalled = 0;
                _timesBeforeStepWasCalled = 0;
                _timesAfterStepWasCalled = 0;
                _timesAfterScenarioWasCalled = 0;
                _beforeRun = 0;
                _afterRun = 0;
                _beforeFeature = 0;
                _afterFeature = 0;

                CreateBasicConfiguration()
                .SetAssemblies(new[] { GetType().Assembly.Location })
                        .SetScenarioFiles(new[] { TestFeatures.FeatureWithScenarioBackground })
                        .SetFilter(new StoryRunnerFilter(StoryRunnerFilter.MatchAnything, GetType().Name, StoryRunnerFilter.MatchAnything))
                        .Build()
                        .Run();
            }

            [Given("this background section declaration")]
            [Given("this one")]
            public void background_step()
            {
                _backgroundStepWasCalledButNotBeforeScenario = _timesBeforeScenarioWasCalled == 0;
                _backgroundStepWasCalledButNotBeforeStep = _timesBeforeStepWasCalled == 0;
            }

            [Given("this scenario under the context of a background section")]
            [When("the scenario with a background section is executed")]
            [Then("the background section steps should be called before this scenario")]
            public void Scenario_step()
            {

            }

            [Test]
            public void When_background_step_is_called_beforeScenario_should_not_be_called()
            {
                Assert.IsTrue(_backgroundStepWasCalledButNotBeforeScenario);
            }

            [Test]
            public void a_background_step_Should_not_trigger_a_before_step()
            {
                Assert.IsTrue(_backgroundStepWasCalledButNotBeforeStep);
            }

            [Test]
            public void Should_call_BeforeRun_once()
            {
                Assert.AreEqual(1, _beforeRun);
            }

            [Test]
            public void Should_call_AfterRun_once()
            {
                Assert.AreEqual(1, _afterRun);
            }

            [Test]
            public void Should_call_BeforeFeature()
            {
                Assert.AreEqual(1, _beforeFeature);
            }

            [Test]
            public void Should_call_AfterFeature()
            {
                Assert.AreEqual(1, _afterFeature);
            }

            [Test]
            public void ShouldCallBeforeScenarioOncePerScenario()
            {
                Assert.That(_timesBeforeScenarioWasCalled, Is.EqualTo(1));
            }

            [Test]
            public void ShouldCallAfterScenarioOncePerScenario()
            {
                Assert.That(_timesAfterScenarioWasCalled, Is.EqualTo(1));
            }

            [Test]
            public void ShouldCallBeforeStepOncePerStep()
            {
                Assert.That(_timesBeforeStepWasCalled, Is.EqualTo(3));
            }

            [Test]
            public void ShouldCallAfterStepOncePerStep()
            {
                Assert.That(_timesAfterStepWasCalled, Is.EqualTo(3));
            }
        }


        [TestFixture, Hooks.Hooks, ActionSteps]
        public class When_Running_step_where_feature_and_scenario_has_tags : TextRunnerSpec
        {
            private static List<string> _tagsBeforeFeature = new List<string>();
            private static List<string> _tagsAfterFeature = new List<string>();
            private static List<string> _tagsBeforeScenario = new List<string>();
            private static List<string> _tagsAfterScenario = new List<string>();
            private static List<string> _tagsBeforeStep = new List<string>();
            private static List<string> _tagsAfterStep = new List<string>();

            [Hooks.BeforeFeature]
            public void OnBeforeFeature()
            {
                _tagsBeforeFeature.AddRange(FeatureContext.Current.Tags);
            }
            [Hooks.AfterFeature]
            public void OnAfterFeature()
            {
                _tagsAfterFeature.AddRange(FeatureContext.Current.Tags);
            }
            [Hooks.BeforeScenario]
            public void OnBeforeScenario()
            {
                _tagsBeforeScenario.AddRange(ScenarioContext.Current.Tags);
            }
            [Hooks.AfterScenario]
            public void OnAfterScenario()
            {
                _tagsAfterScenario.AddRange(ScenarioContext.Current.Tags);
            }
            [Hooks.BeforeStep]
            public void OnBeforeStep()
            {
                _tagsBeforeStep.AddRange(StepContext.Current.Tags);
            }
            [Hooks.AfterStep]
            public void OnAfterStep()
            {
                _tagsAfterStep.AddRange(StepContext.Current.Tags);
            }

            [TestFixtureSetUp]
            public void Setup()
            {
                _tagsBeforeFeature.Clear();
                _tagsAfterFeature.Clear();
                _tagsBeforeScenario.Clear();
                _tagsAfterScenario.Clear();
                _tagsBeforeStep.Clear();
                _tagsAfterStep.Clear();
                ConfigurationNoAppDomain.New
                .SetAssemblies(new[] { GetType().Assembly.Location })
                        .SetScenarioFiles(new[] { TestFeatures.FeatureWithTags })
                        .SetFilter(new StoryRunnerFilter(".", GetType().Name, "."))
                        .Build()
                        .Run();
            }

            [Given("my name is Morgan")]
            public void Given_step()
            { }

            [When("I'm greeted")]
            public void WhenGreeted()
            { }

            [Then("I should be greeted with “Hello, $name!”")]
            public void ThenGreeted(string name)
            {
                Assert.AreEqual("Morgan", name);
            }

            [Test]
            public void FeatureContexct_should_have_tags()
            {
                CollectionAssert.IsNotEmpty(_tagsBeforeFeature);
                CollectionAssert.IsNotEmpty(_tagsAfterFeature);
            }

            [Test]
            public void ScenarioContexct_should_have_tags()
            {
                CollectionAssert.IsNotEmpty(_tagsBeforeScenario);
                CollectionAssert.IsNotEmpty(_tagsAfterScenario);
            }

            [Test]
            public void StepContexct_should_have_tags()
            {
                CollectionAssert.IsNotEmpty(_tagsBeforeStep);
                CollectionAssert.IsNotEmpty(_tagsAfterStep);
            }
        }
    }
}