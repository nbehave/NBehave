using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class ScenarioClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Scenario;
        }

        public override IList<ClassificationSpan> Classify(ParserEvent event3)
        {
            ITextSnapshotLine textSnapshotLine = event3.Snapshot.GetLineFromLineNumber(event3.Line - 1);

            List<ClassificationSpan> spans = new List<ClassificationSpan>();
            try
            {
                ClassificationSpan classificationSpan = GetKeywordSpan(textSnapshotLine, event3.Keyword, event3.Snapshot);
                spans.Add(classificationSpan);
                ClassificationSpan titleSpan = GetTitleSpan(textSnapshotLine, event3.Snapshot, ClassificationRegistry.ScenarioTitle);
                spans.Add(titleSpan);
            }
            catch (Exception)
            {
            }

            return spans;
        }
    }
}