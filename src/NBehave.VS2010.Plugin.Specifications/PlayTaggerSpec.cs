using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.Editor.Glyphs;
using NBehave.VS2010.Plugin.Specifications.MockObjects;
using NBehave.VS2010.Plugin.Tagging;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.VS2010.Plugin.Specifications
{
    [TestFixture]
    public class PlayTaggerSpec
    {
        private ITextBuffer buffer;
        private ITagger<PlayGlyphTag> playTagger;
        private string featureFileContent;

        private void TestInitialise(string gherkinFileLocation)
        {
            var registry = MockRepository.GenerateMock<IClassificationTypeRegistryService>();
            registry.Stub(service => service.GetClassificationType(null))
                .IgnoreArguments()
                .WhenCalled(invocation =>
                {
                    invocation.ReturnValue = new MockClassificationType
                    {
                        Classification = (string)invocation.Arguments.First()
                    };
                });

            buffer = MockRepository.GenerateMock<ITextBuffer>();
            buffer.Stub(textBuffer => textBuffer.Properties).Return(new PropertyCollection());

            featureFileContent = new StreamReader(gherkinFileLocation).ReadToEnd();
            buffer.Stub(b => b.CurrentSnapshot).Return(new MockTextSnapshot(featureFileContent));

            //var gherkinFileEditorParser = new GherkinFileEditorParser();
            //gherkinFileEditorParser.InitialiseWithBuffer(_buffer);
            //_buffer.Properties.AddProperty(typeof(GherkinFileEditorParser), gherkinFileEditorParser);

            playTagger = new PlayTagger(new TokenParser(buffer));

            //gherkinFileEditorParser.FirstParse();
        }

        [Test]
        public void ShouldClassifyScenarioWithExamples()
        {
            TestInitialise("Features/gherkin.feature");
            var snapShotSpan = new SnapshotSpan(new MockTextSnapshot(featureFileContent), new Span(0, 15));
            var spanCollection = new NormalizedSnapshotSpanCollection(new[] { snapShotSpan });
            var tags = playTagger.GetTags(spanCollection);
            var tag = tags.First(span => span.Span.GetText().StartsWith("  Scenario: SC1")).Tag;

            Assert.AreEqual("Scenario: SC1" + Environment.NewLine +
                            "  Given numbers [left] and [right]" + Environment.NewLine +
                            "  When I add the numbers" + Environment.NewLine +
                            "  Then the sum is [sum]" + Environment.NewLine +
                            "Examples:" + Environment.NewLine +
                            "     | left | right | sum |" + Environment.NewLine +
                            "     | 1 | 2 | 3 |" + Environment.NewLine +
                            "     | 3 | 4 | 7 |", tag.GetText());
        }

        [Test]
        public void ShouldClassifyScenarioWithInlineTable()
        {
            TestInitialise("Features/gherkin.feature");

            var snapShotSpan = new SnapshotSpan(new MockTextSnapshot(featureFileContent), new Span(0, 15));
            var spanCollection = new NormalizedSnapshotSpanCollection(new[] { snapShotSpan });
            var tags = playTagger.GetTags(spanCollection);
            var tag = tags.First(span => span.Span.GetText().StartsWith("  Scenario: inline table")).Tag;

            Assert.AreEqual("Scenario: inline table" + Environment.NewLine +
                            "  Given the following people exists:" + Environment.NewLine +
                            "     | Name | Country |" + Environment.NewLine +
                            "     | Morgan Persson | Sweden |" + Environment.NewLine +
                            "     | Jimmy Nilsson | Sweden |" + Environment.NewLine +
                            "     | Jimmy bogard | USA |" + Environment.NewLine +
                            "  When I search for people in sweden" + Environment.NewLine +
                            "  Then I should get:" + Environment.NewLine +
                            "     | Name |" + Environment.NewLine +
                            "     | Morgan Persson |" + Environment.NewLine +
                            "     | Jimmy Nilsson |", tag.GetText());
        }

        [Test]
        public void ShouldClassifyScenario()
        {
            TestInitialise("Features/gherkin.feature");
            var snapShotSpan = new SnapshotSpan(new MockTextSnapshot(featureFileContent), new Span(0, 15));
            var spanCollection = new NormalizedSnapshotSpanCollection(new[] { snapShotSpan });
            var tags = playTagger.GetTags(spanCollection);
            var tag = tags.First(_ => _.Span.GetText().StartsWith("  Scenario: SC2")).Tag;
            Assert.AreEqual("Scenario: SC2" + Environment.NewLine +
                            "  Given something" + Environment.NewLine +
                            "  When some event occurs" + Environment.NewLine +
                            "  Then there is some outcome", tag.GetText());
        }

        [Test]
        public void ShouldClassifyScenarioWithMultipleInlineTables()
        {
            TestInitialise("Features/Table.feature");
            var snapShotSpan = new SnapshotSpan(new MockTextSnapshot(featureFileContent), new Span(0, 36));
            var spanCollection = new NormalizedSnapshotSpanCollection(new[] { snapShotSpan });

            var tags = playTagger.GetTags(spanCollection);
            var tag = tags.First(span => span.Span.GetText().StartsWith("Scenario: a table")).Tag;

            Assert.AreEqual("Scenario: a table" + Environment.NewLine +
                            "  Given a list of people:" + Environment.NewLine +
                            "     | name | country |" + Environment.NewLine +
                            "     | Morgan Persson | Sweden |" + Environment.NewLine +
                            "     | Jimmy Nilsson | Sweden |" + Environment.NewLine +
                            "     | Jimmy Bogard | USA |" + Environment.NewLine +
                            "  When I search for people from Sweden" + Environment.NewLine +
                            "  Then I should find:" + Environment.NewLine +
                            "     | Name | Country |" + Environment.NewLine +
                            "     | Morgan Persson | Sweden |" + Environment.NewLine +
                            "     | Jimmy Nilsson | Sweden |", tag.GetText());
        }
    }
}
