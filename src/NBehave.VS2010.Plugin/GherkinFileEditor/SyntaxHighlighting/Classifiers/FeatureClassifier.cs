using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class FeatureClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Feature;
        }

        public override IList<ClassificationSpan> Classify(ParserEvent event3)
        {
            ITextSnapshotLine textSnapshotLine = event3.Snapshot.GetLineFromLineNumber(event3.Line - 1);

            return new[]
                       {
                           GetKeywordSpan(textSnapshotLine, event3.Keyword, event3.Snapshot),
                           GetTitleSpan(textSnapshotLine, event3.Snapshot),
                           GetDescriptionSpan(event3.Line, event3.Description, event3.Snapshot),
                       };

        }

        private ClassificationSpan GetDescriptionSpan(int line, string description, ITextSnapshot snapshot)
        {
            int descriptionStartPosition = snapshot.GetLineFromLineNumber(line).Start.Position;

            int descriptionEndPosition = snapshot.GetLineFromLineNumber(
                description.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Count() + line).Start.Position;

            var descriptionSpan = new Span(descriptionStartPosition, descriptionEndPosition - descriptionStartPosition);


            return new ClassificationSpan(new SnapshotSpan(snapshot, descriptionSpan), ClassificationRegistry.Description);
        }
    }
}