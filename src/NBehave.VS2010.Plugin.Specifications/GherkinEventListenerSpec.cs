using NBehave.Internal.Gherkin;
using NBehave.VS2010.Plugin.Tagging;
using NUnit.Framework;

namespace NBehave.VS2010.Plugin.Specifications
{
    [TestFixture]
    public class GherkinEventListenerSpec
    {
        [Test]
        public void Should_add_tag_event()
        {
            var l = new GherkinEventListener();
            l.Tag(new Token("@tag", new LineInFile(1)));
            Assert.AreEqual(GherkinTokenType.Tag, l.Events[0].GherkinTokenType);
            Assert.AreEqual("@tag", l.Events[0].Tokens[0].Content);
        }

        [Test]
        public void Should_add_tag_events_following_each_other_to_the_same_event()
        {
            var l = new GherkinEventListener();
            l.Tag(new Token("@tag1", new LineInFile(1)));
            l.Tag(new Token("@tag2", new LineInFile(1)));
            Assert.AreEqual(1, l.Events.Count);
            Assert.AreEqual(GherkinTokenType.Tag, l.Events[0].GherkinTokenType);
            Assert.AreEqual("@tag1", l.Events[0].Tokens[0].Content);
            Assert.AreEqual("@tag2", l.Events[0].Tokens[1].Content);
        }
        [Test]
        public void Should_not_add_tag_thats_only_an_at_char()
        {
            // GurkBurk bug?
            var l = new GherkinEventListener();
            l.Tag(new Token("@", new LineInFile(1)));
            Assert.AreEqual(0, l.Events.Count);
        }

    }
}