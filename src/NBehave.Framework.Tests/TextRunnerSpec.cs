using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class TextRunnerSpec
    {
        private TextRunner CreateTextRunner()
        {
            var writer = new StreamWriter(new MemoryStream());
            var listener = new TextWriterEventListener(writer);
            return CreateTextRunner(listener);
        }

        private TextRunner CreateTextRunner(IEventListener listener)
        {
            return new TextRunner(listener);
        }

        [TestFixture]
        public class WhenRunningPlainTextScenarios : TextRunnerSpec
        {
            private TextRunner _runner;

            [SetUp]
            public void SetUp()
            {
                _runner = CreateTextRunner();
                LoadAssembly();
            }

            private void LoadAssembly()
            {
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
            }

            [Test]
            public void ShouldFindGivenActionStepInAssembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("my name is Axel"), Is.True);
            }

            [Test]
            public void ShouldFindWhenActionStepInAssembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("I'm greeted"), Is.True);
            }

            [Test]
            public void ShouldFindThenActionStepInAssembly()
            {
                Assert.That(_runner.ActionCatalog.ActionExists("I should be greeted with “Hello, Axel!”"), Is.True);
            }
            
            [Test]
            public void ShouldRunScenariosInTextFile()
            {
                _runner.Load(new[] { TestFeatures.ScenariosWithoutFeature });
                var result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(2));
            }
            
            [Test]
            public void ShouldGetResultOfRunningScenariosInTextFile()
            {
                _runner.Load(new[] { TestFeatures.ScenariosWithoutFeature });
                var results = _runner.Run();
                Assert.That(results.NumberOfThemes, Is.EqualTo(0));
                Assert.That(results.NumberOfStories, Is.EqualTo(1));
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(2));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
            }

            [Test]
            public void ShouldGetCorrectErrormessageFromFailedScenario()
            {
                _runner.Load(new[] { TestFeatures.FeatureWithFailingStep });
                var results = _runner.Run();
                Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(1));
                Assert.That(results.ScenarioResults[0].Message.StartsWith("NUnit.Framework.AssertionException :"), Is.True);
            }

            [Test]
            public void ShouldMarkFailingStepAsFailedInOutput()
            {
                var writer = new StringWriter();
                var listener = new TextWriterEventListener(writer);
                _runner = CreateTextRunner(listener);
                LoadAssembly();
                _runner.Load(new[] { TestFeatures.FeatureWithFailingStep });
                _runner.Run();
                StringAssert.Contains("Then I should be greeted with “Hello, Scott!” - FAILED", writer.ToString());
            }

            [Test]
            public void ShouldExecuteMoreThanOneScenarioInTextFile()
            {
                var writer = new StreamWriter(new MemoryStream());
                var listener = new TextWriterEventListener(writer);
                _runner = CreateTextRunner(listener);
                _runner.LoadAssembly("TestPlainTextAssembly.dll");
                _runner.Load(new[] { TestFeatures.FeatureWithManyScenarios });
                var results = _runner.Run();
                Assert.That(results.NumberOfScenariosFound, Is.EqualTo(2));
                Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
            }

            [Test]
            public void ShouldRunScenarioInTextFileWithScenarioTitle()
            {
                _runner.Load(new[] { TestFeatures.ScenariosWithoutFeature });
                var results = _runner.Run();

                Assert.That(results.ScenarioResults[0].ScenarioTitle, Is.EqualTo("greeting Morgan"));
                Assert.That(results.ScenarioResults[0].Result, Is.TypeOf(typeof(Passed)));
            }

            [Test]
            public void ShouldRunTextScenarioWhithNewlinesInGiven()
            {
                _runner.Load(new [] { TestFeatures.FeatureWithNewLineInGivenClause });
                var result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void ShouldSetScenarioPendingIfActionGivenInTokenStringDoesntExist()
            {
                _runner.Load(new [] { TestFeatures.ScenarioWithNoActionSteps });
                var result = _runner.Run();
                Assert.That(result.NumberOfPendingScenarios, Is.EqualTo(1));
            }

            [Test]
            public void ShouldListAllPendingActionSteps()
            {
                _runner.Load(new [] { TestFeatures.ScenarioWithNoActionSteps });
                var result = _runner.Run();
                StringAssert.Contains("No matching Action found for \"Given something that has no ActionStep\"", result.ScenarioResults[0].Message);
                StringAssert.Contains("No matching Action found for \"And something else that has no ActionStep\"", result.ScenarioResults[0].Message);
            }

            [Test]
            public void ShouldUseWildcardAndRunAllScenariosInAllMatchingTextFiles()
            {
                _runner.Load(new[] { @"Features\\Feature*.feature" });
                var result = _runner.Run();
                Assert.That(result.NumberOfPassingScenarios, Is.EqualTo(4));
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
                var runner = new TextRunner(listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new [] { TestFeatures.FeatureWithFailingStep });
                runner.Run();
                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            [Test]
            public void ShouldFindOneFailedActionStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//actionStep[@outcome='failed']");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Test]
            public void ShouldFindTwoPassedActionStep()
            {
                var storyNodes = _xmlOut.SelectNodes("//actionStep[@outcome='passed']");
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
                var runner = new TextRunner(listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new[] { TestFeatures.FeatureWithManyScenarios });
                runner.Run();
                _xmlOut = new XmlDocument();
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                _xmlOut.Load(writer.BaseStream);
            }

            [Test]
            public void ShouldFindOneStory()
            {
                var storyNodes = _xmlOut.SelectNodes("//story");
                Assert.That(storyNodes.Count, Is.EqualTo(1));
            }

            [Test]
            public void ShouldSetTitleOfStory()
            {
                var storyNodes = _xmlOut.SelectSingleNode("//story").Attributes["name"];

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
        public class WhenRunningPlainTextScenariosWithStory : TextRunnerSpec
        {
            private FeatureResults _result;
            private StringWriter _messages;

            [SetUp]
            public void SetUp()
            {
                _messages = new StringWriter();
                var listener = new TextWriterEventListener(_messages);
                var runner = new TextRunner(listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new []{ TestFeatures.FeatureNamedStory });
                _result = runner.Run();
            }

            [Test]
            public void ShouldSetStoryTitleOnResult()
            {
                Assert.That(_result.ScenarioResults[0].FeatureTitle, Is.EqualTo("Greeting system"));
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
                Assert.That(_result.ScenarioResults[0].ScenarioTitle, Is.EqualTo("Greeting someone"));
            }
        }

        [TestFixture]
        public class WhenRunningPlainTextScenariosWithStoryEventsRaised : TextRunnerSpec
        {
            private IEventListener _listener;

            [TestFixtureSetUp]
            public void EstablishContext()
            {
                _listener = MockRepository.GenerateMock<IEventListener>();
                var runner = new TextRunner(_listener);
                runner.LoadAssembly("TestPlainTextAssembly.dll");

                runner.Load(new []{ TestFeatures.FeatureNamedStory });
                runner.Run();
            }

            [Test]
            public void ShouldGetStoryCreatedEventWithTitle()
            {
                _listener.AssertWasCalled(l => l.FeatureCreated("Greeting system"));
            }

            [Test]
            public void ShouldGetStoryNarrative()
            {
                var args = _listener.GetArgumentsForCallsMadeOn(l => l.FeatureNarrative(null), opt => opt.IgnoreArguments());
                var arg = args[0][0] as string;
                StringAssert.Contains("As a", arg);
                StringAssert.Contains("I want", arg);
                StringAssert.Contains("So that", arg);
            }

            [Test]
            public void ShouldGetScenarioCreatedEventWithTitle()
            {
                _listener.AssertWasCalled(l => l.ScenarioCreated("Greeting someone"));
            }
        }
    
        [TestFixture, ActionSteps]
        public class WhenRunningPlainTextScenarioInSwedish : TextRunnerSpec
        {
            private TextRunner _runner;

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

            [SetUp]
            public void SetUp()
            {
                _runner = new TextRunner(Framework.EventListeners.EventListeners.NullEventListener());
                _runner.LoadAssembly(GetType().Assembly);
                _runner.Load(new[] { TestFeatures.FeatureInSwedish });
                _featureResults = _runner.Run();
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
    }
}
