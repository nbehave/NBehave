using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class FeatureClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Feature;
        }

        public override void RegisterClassificationDefinitions()
        {
            Register(parserEvent => GetKeywordSpan(parserEvent));
            Register(parserEvent => GetTitleSpan(parserEvent, ClassificationRegistry.FeatureTitle));
            Register(parserEvent => GetDescriptionSpan(parserEvent.Line, parserEvent.Description, parserEvent.Snapshot));   
        }

        private ClassificationSpan GetDescriptionSpan(int line, string description, ITextSnapshot snapshot)
        {
            int descriptionStartPosition = snapshot.GetLineFromLineNumber(line).Start.Position;


            int lineNumber = description.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Count() + line;

            if (snapshot.LineCount <= lineNumber)
                return null;

            int descriptionEndPosition = snapshot.GetLineFromLineNumber(
                lineNumber).Start.Position;

            var descriptionSpan = new Span(descriptionStartPosition, descriptionEndPosition - descriptionStartPosition);


            return new ClassificationSpan(new SnapshotSpan(snapshot, descriptionSpan), ClassificationRegistry.Description);
        }
    }
}