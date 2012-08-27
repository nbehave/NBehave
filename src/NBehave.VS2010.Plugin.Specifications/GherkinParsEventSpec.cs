using NBehave.Gherkin;
using NBehave.VS2010.Plugin.Tagging;
using NUnit.Framework;

namespace NBehave.VS2010.Plugin.Specifications
{
    [TestFixture]
    public class GherkinParsEventSpec
    {
        [Test]
        public void Should_be_equal_if_same_evt_and_no_tags()
        {
            var e1 = new GherkinParseEvent(GherkinTokenType.Feature);
            var e2 = new GherkinParseEvent(GherkinTokenType.Feature);

            Assert.AreEqual(e1, e2);
        }

        [Test]
        public void Should_be_equal_if_all_tokens_are_equal()
        {
            var e1 = new GherkinParseEvent(GherkinTokenType.Feature, new Token("aaa", new LineInFile(1)), new Token("bbb", new LineInFile(2)));
            var e2 = new GherkinParseEvent(GherkinTokenType.Feature, new Token("aaa", new LineInFile(1)), new Token("bbb", new LineInFile(2)));

            Assert.AreEqual(e1, e2);
        }

        [Test]
        public void Should_not_be_equal_if_all_tokens_differ_by_line_number()
        {
            var e1 = new GherkinParseEvent(GherkinTokenType.Feature, new Token("aaa", new LineInFile(1)));
            var e2 = new GherkinParseEvent(GherkinTokenType.Feature, new Token("aaa", new LineInFile(2)));

            Assert.AreNotEqual(e1, e2);
        }

        [Test]
        public void Should_not_be_equal_if_all_tokens_differ_by_text()
        {
            var e1 = new GherkinParseEvent(GherkinTokenType.Feature, new Token("aaa", new LineInFile(1)));
            var e2 = new GherkinParseEvent(GherkinTokenType.Feature, new Token("bbb", new LineInFile(1)));

            Assert.AreNotEqual(e1, e2);
        }

        [Test]
        public void Should_not_be_equal_if_differnet_number_of_tokens()
        {
            var e1 = new GherkinParseEvent(GherkinTokenType.Feature);
            var e2 = new GherkinParseEvent(GherkinTokenType.Feature, new Token("x", new LineInFile(1)));

            Assert.AreNotEqual(e1, e2);
        }
    }
}
