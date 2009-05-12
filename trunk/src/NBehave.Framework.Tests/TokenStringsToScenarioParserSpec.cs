using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class TokenStringsToScenarioParserSpec
    {
        public class WhenListOfTokenStringContainsOneStory : TokenStringsToScenarioParserSpec
        {
            private TokenStringsToScenarioParser _parser;

            [SetUp]
            public void GivenTheseConditions()
            {
                _parser = new TokenStringsToScenarioParser();

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

        public class WhenListOfTokenStringContainsTwoStories : TokenStringsToScenarioParserSpec
        {
            private TokenStringsToScenarioParser _parser;

            [SetUp]
            public void GivenTheseConditions()
            {
                _parser = new TokenStringsToScenarioParser();

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
    }
}