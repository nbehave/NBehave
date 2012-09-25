using System.Linq;
using Microsoft.VisualStudio.Text;
using NBehave.Gherkin;
using NBehave.VS2010.Plugin.Specifications.MockObjects;
using NBehave.VS2010.Plugin.Tagging;
using NUnit.Framework;

namespace NBehave.VS2010.Plugin.Specifications
{
    [TestFixture]
    public class GherkinStepTaggerSpec
    {
        [Test]
        public void Should_tag_feature_with_title()
        {
            var gherkinStepTagger = new GherkinStepTagger();
            var line = new LineInFile(0);
            var evt = new GherkinParseEvent(GherkinTokenType.Feature,
                new Token("Feature", line), new Token("title", line), new Token("foo", line));

            ITextSnapshot snapshot = new MockTextSnapshot("Feature: title");
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            var tags = gherkinStepTagger.CreateTags(new[] { evt }, span).ToArray();
            Assert.AreEqual(GherkinTokenType.Feature, tags[0].Tag.Type);
            Assert.AreEqual("Feature", tags[0].Span.GetText());
            Assert.AreEqual(GherkinTokenType.FeatureTitle, tags[1].Tag.Type);
            Assert.AreEqual("title", tags[1].Span.GetText());
        }

        [Test]
        public void Should_tag_feature_narrative()
        {
            var gherkinStepTagger = new GherkinStepTagger();
            var line = new LineInFile(0);
            var evt = new GherkinParseEvent(GherkinTokenType.Feature,
                new Token("Feature", line), new Token("title", line), new Token("foo", line));

            ITextSnapshot snapshot = new MockTextSnapshot("foo");
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            var tags = gherkinStepTagger.CreateTags(new[] { evt }, span).ToArray();
            Assert.AreEqual(GherkinTokenType.FeatureDescription, tags[0].Tag.Type);
            Assert.AreEqual("foo", tags[0].Span.GetText());
        }

        [Test]
        public void Should_tag_feature_narrative_spanning_multiple_lines()
        {
            var gherkinStepTagger = new GherkinStepTagger();
            var line = new LineInFile(0);
            var evt = new GherkinParseEvent(GherkinTokenType.Feature,
                new Token("Feature", line), new Token("title", line), new Token("foo\r\n\tbar", line));

            ITextSnapshot snapshot = new MockTextSnapshot("\tbar\r\n");
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            var tags = gherkinStepTagger.CreateTags(new[] { evt }, span).ToArray();
            Assert.AreEqual(GherkinTokenType.FeatureDescription, tags[0].Tag.Type);
            Assert.AreEqual("bar", tags[0].Span.GetText());
        }

        [Test]
        public void Should_tag_scenario_with_title()
        {
            var gherkinStepTagger = new GherkinStepTagger();
            var line = new LineInFile(0);
            var evt = new GherkinParseEvent(GherkinTokenType.Scenario, new Token("Scenario", line), new Token("title", line));

            ITextSnapshot snapshot = new MockTextSnapshot("Scenario: title");
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            var tags = gherkinStepTagger.CreateTags(new[] { evt }, span).ToArray();
            Assert.AreEqual(GherkinTokenType.Scenario, tags[0].Tag.Type);
            Assert.AreEqual("Scenario", tags[0].Span.GetText());
            Assert.AreEqual(GherkinTokenType.ScenarioTitle, tags[1].Tag.Type);
            Assert.AreEqual("title", tags[1].Span.GetText());
        }

        [Test]
        public void Should_tag_scenario_with_title_spanning_multiple_lines()
        {
            var gherkinStepTagger = new GherkinStepTagger();
            var line = new LineInFile(0);
            var evt = new GherkinParseEvent(GherkinTokenType.Scenario, new Token("Scenario", line), new Token("title\n\tbar", line));

            ITextSnapshot snapshot = new MockTextSnapshot("bar");
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            var tags = gherkinStepTagger.CreateTags(new[] { evt }, span).ToArray();
            Assert.AreEqual(GherkinTokenType.ScenarioTitle, tags[0].Tag.Type);
            Assert.AreEqual("bar", tags[0].Span.GetText());
        }

        [Test]
        public void Should_tag_tags()
        {
            var gherkinStepTagger = new GherkinStepTagger();
            var line = new LineInFile(0);
            var evt = new GherkinParseEvent(GherkinTokenType.Tag, new Token("Tag", line), new Token("@foo", line), new Token("@bar", line), new Token("@baz", line));

            ITextSnapshot snapshot = new MockTextSnapshot("@foo @bar @baz");
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            var tags = gherkinStepTagger.CreateTags(new[] { evt }, span).ToArray();
            Assert.AreEqual(GherkinTokenType.Tag, tags[0].Tag.Type);
            Assert.AreEqual("@foo", tags[0].Span.GetText());
            Assert.AreEqual(GherkinTokenType.Tag, tags[1].Tag.Type);
            Assert.AreEqual("@bar", tags[1].Span.GetText());
            Assert.AreEqual(GherkinTokenType.Tag, tags[2].Tag.Type);
            Assert.AreEqual("@baz", tags[2].Span.GetText());
        }
    }
}