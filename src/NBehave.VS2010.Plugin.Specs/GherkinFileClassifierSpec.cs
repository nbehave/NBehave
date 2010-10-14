using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.GherkinFileEditor;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.VS2010.Plugin.Specs
{
    [TestFixture]
    public class GherkinFileClassifierSpec
    {
        private GherkinFileClassifier _gherkinFileClassifier;
        private ITextSnapshot _snapshot;
        private ITextBuffer _buffer;

        [SetUp]
        public void Setup()
        {
            var registry = MockRepository.GenerateMock<IClassificationTypeRegistryService>();
            registry.Stub(service => service.GetClassificationType(null)).IgnoreArguments().WhenCalled(invocation =>
                                                                                                           {
                                                                                                               invocation
                                                                                                                   .
                                                                                                                   ReturnValue
                                                                                                                   = new MockClassificationType
                                                                                                                         {
                                                                                                                             Classification
                                                                                                                                 =
                                                                                                                                 (
                                                                                                                                 string
                                                                                                                                 )
                                                                                                                                 invocation
                                                                                                                                     .
                                                                                                                                     Arguments
                                                                                                                                     .
                                                                                                                                     First
                                                                                                                                     ()
                                                                                                                         };
                                                                                                           });

            _gherkinFileClassifier = new GherkinFileClassifier
            {
                ClassificationRegistry = new GherkinFileEditorClassifications
                {
                    ClassificationRegistry = registry
                },
                GherkinFileEditorParserFactory = new GherkinFileEditorParserFactory
                {
                    GherkinFileEditorParser = new GherkinFileEditorParser()
                }
            };

            _buffer = MockRepository.GenerateMock<ITextBuffer>();
            _buffer.Stub(textBuffer => textBuffer.Properties).Return(new PropertyCollection());
            _snapshot = MockRepository.GenerateMock<ITextSnapshot>();

            
            
        }

        [Test]
        public void ShouldClassifyFeatures()
        {
            var gherkinFile = new StreamReader("Features/gherkin.feature").ReadToEnd();

            _buffer.Stub(buffer => buffer.CurrentSnapshot).Return(new MockTextSnapshot(gherkinFile));

            _gherkinFileClassifier.InitialiseWithBuffer(_buffer);

            var featureSpans = _gherkinFileClassifier.GetClassificationSpans(new SnapshotSpan()).Where(span => span.ClassificationType.IsOfType("gherkin.featuretitle"));
            foreach (var classificationSpan in featureSpans)
            {
                Assert.That(classificationSpan.Span.GetText(), Is.EqualTo("Feature"));
            }
        }
    }
}
