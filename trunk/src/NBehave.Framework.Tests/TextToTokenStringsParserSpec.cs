using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class TextToTokenStringsParserSpec
    {
        private TextToTokenStringsParser _tokenStringsParser;
        private ActionStepAlias _actionStepAlias;

        [SetUp]
        public void Setup()
        {
            _actionStepAlias = new ActionStepAlias();
            _tokenStringsParser = new TextToTokenStringsParser(_actionStepAlias);
        }

        [Test]
        public void ShouldFindThreeTokens()
        {
            const string scenario = "Given my name is Morgan\n" +
                                    "When I'm greeted\n" +
                                    "Then I should be greeted with “Hello, Morgan!”";

            _tokenStringsParser.ParseScenario(scenario);

            Assert.That(_tokenStringsParser.TokenStrings.Count, Is.EqualTo(3));
        }

        [Test]
        public void ShouldParseTokenFromText()
        {
            const string scenario = "Given my name is Morgan\n" +
                                    "When I'm greeted\n" +
                                    "Then I should be greeted with “Hello, Morgan!”";

            _tokenStringsParser.ParseScenario(scenario);

            Assert.That(_tokenStringsParser.TokenStrings[0], Is.EqualTo("Given my name is Morgan"));
        }

        [Test]
        public void ShouldParseFirstTokenFromTextWhenTokenHasNewLine()
        {
            const string scenario = "Given my\n" +
                                    "name is Morgan\n" +
                                    "When I'm greeted\n" +
                                    "Then I should be greeted with “Hello, Morgan!”";
            _tokenStringsParser.ParseScenario(scenario);

            Assert.That(_tokenStringsParser.TokenStrings[0], Is.EqualTo("Given my\nname is Morgan"));
        }

        [Test]
        public void ShouldParseLastTokenFromTextWhenTokenHasNewLine()
        {
            const string scenario = "Given my\n" +
                                    "name is Morgan\n" +
                                    "When I'm greeted\n" +
                                    "Then I should be greeted with “Hello, Morgan!”";
            _tokenStringsParser.ParseScenario(scenario);

            Assert.That(_tokenStringsParser.TokenStrings[2],
                        Is.EqualTo("Then I should be greeted with “Hello, Morgan!”"));
        }

        [Test]
        public void ShouldParseTokenFromTextUsingAlias()
        {
            const string scenario = "Given my name is Morgan\n" +
                                    "When I'm greeted\n" +
                                    "And I should be greeted with “Hello, Morgan!”";

            _actionStepAlias.AddDefaultAlias(new[] { "And" }, "Given");
            _tokenStringsParser.ParseScenario(scenario);

            Assert.That(_tokenStringsParser.TokenStrings[2], Is.EqualTo("And I should be greeted with “Hello, Morgan!”"));
        }

        [Test]
        public void Should_parse_TokenString_on_multiple_lines()
        {
            const string scenario = "Given my name is\nMorgan\nPersson\n" +
                                 "When I'm greeted\n" +
                                 "Then I should be greeted with “Hello, Morgan Persson!”";

            _tokenStringsParser.ParseScenario(scenario);

            Assert.That(_tokenStringsParser.TokenStrings.Count, Is.EqualTo(3));
            Assert.That(_tokenStringsParser.TokenStrings[0], Is.EqualTo("Given my name is\nMorgan\nPersson"));
        }
    }
}