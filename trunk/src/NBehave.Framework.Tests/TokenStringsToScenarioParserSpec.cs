using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class TokenStringsToScenarioParserSpec
    {
            private TokenStringsToScenarioParser _parser;

            private void CreateParser()
            {
                var actionStep = new ActionStep();
                _parser = new TokenStringsToScenarioParser(actionStep);
            }

        public class WhenListOfTokenStringContainsOneScenario : TokenStringsToScenarioParserSpec
        {
            [SetUp]
            public void GivenTheseConditions()
            {
                CreateParser();
                var tokens = new List<string>
                                 {
                                     "Given my name is Morgan",
                                     "When I'm greeted",
                                     "Then I should be greeted with “Hello, Morgan!”"
                                 };
                _parser.ParseTokensToScenarios(tokens);
            }

            [Test]
            public void ShouldParseTokensToScenario()
            {
                Assert.That(_parser.Scenarios.Count, Is.EqualTo(1));
            }
        }

        public class WhenListOfTokenStringContainsTwoScenarios : TokenStringsToScenarioParserSpec
        {
            [SetUp]
            public void GivenTheseConditions()
            {
                CreateParser();
                
                var tokens = new List<string>
                                 {
                                     "Given my name is Morgan",
                                     "When I'm greeted",
                                     "Then I should be greeted with “Hello, Morgan!”",
                                     "Given my name is Axel",
                                     "When I'm greeted",
                                     "Then I should be greeted with “Hello, Axel!”"
                                 };
                _parser.ParseTokensToScenarios(tokens);
            }

            [Test]
            public void ShouldParseTokensToScenario()
            {
                Assert.That(_parser.Scenarios.Count, Is.EqualTo(2));
            }
        }

        public class WhenListOfTokenStringContainsTwoScenariosAndStory : TokenStringsToScenarioParserSpec
        {
            [SetUp]
            public void GivenTheseConditions()
            {
                CreateParser();

                var tokens = new List<string>
                                 {
                                     "Story: Greeting story",
                                     "Scenario: Hello Morgan",
                                     "Given my name is Morgan",
                                     "When I'm greeted",
                                     "Then I should be greeted with “Hello, Morgan!”",

                                     "Scenario: Hello Axel",
                                     "Given my name is Axel",
                                     "When I'm greeted",
                                     "Then I should be greeted with “Hello, Axel!”"
                                 };
                _parser.ParseTokensToScenarios(tokens);
            }

            [Test]
            public void ShouldParseTokensToScenario()
            {
                Assert.That(_parser.Scenarios.Count, Is.EqualTo(2));
            }
        }
    }
}