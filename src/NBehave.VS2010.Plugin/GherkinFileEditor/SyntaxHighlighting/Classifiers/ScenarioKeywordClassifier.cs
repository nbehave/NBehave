using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class ScenarioKeywordClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Scenario;
        }

        public override IList<ClassificationSpan> Classify(ParserEvent event3)
        {
            ITextSnapshotLine textSnapshotLine = event3.Snapshot.GetLineFromLineNumber(event3.Line - 1);

            return new[]
                       {
                           GetKeywordSpan(textSnapshotLine, event3.Keyword, event3.Snapshot), 
                       };
        }
    }
}