using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioStepRunnerSpec
    {
        private ScenarioStepRunner _runner;
        private ActionCatalog _actionCatalog;

        [SetUp]
        public void SetUp()
        {
            var actionStep = new ActionStep();
            var actionStepAlias = new ActionStepAlias();
            var textToTokenStringParser = new TextToTokenStringsParser(actionStepAlias, actionStep);
            _actionCatalog = new ActionCatalog();
            var actionSteprunner = new ActionStepRunner(_actionCatalog);
            _runner = new ScenarioStepRunner(textToTokenStringParser, actionSteprunner, actionStep);
        }

        [Test]
        public void Should_have_result_for_each_step()
        {
            Action<string> action = name => Assert.AreEqual("Morgan", name);
            _actionCatalog.Add(new Regex(@"my name is (?<name>\w+)"), action);

            ScenarioResult scenarioResult = _runner.RunScenario(
                "Given my name is Axel" + Environment.NewLine +
                "And my name is Morgan", 1);

            Assert.AreEqual(2, scenarioResult.ActionStepResults.Count());
        }
    }
}