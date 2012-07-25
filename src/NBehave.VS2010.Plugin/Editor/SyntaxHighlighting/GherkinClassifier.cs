using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.Narrator.Framework;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor.SyntaxHighlighting
{
    [Export(typeof(GherkinClassifier))]
    public class GherkinClassifier
    {
        [Import]
        public GherkinFileEditorClassifications ClassificationRegistry { get; set; }

        public ClassificationSpan CreateFeatureClassification(Feature feature, ITextSnapshot snapshot)
        {
            return CreateClassification(feature.SourceLine, feature.Title, ClassificationRegistry.FeatureTitle, snapshot);
        }

        public IEnumerable<ClassificationSpan> CreateScenarioClassification(Feature feature, ITextSnapshot snapshot)
        {
            return feature.Scenarios
                .Select(scenario => CreateClassification(scenario.SourceLine, scenario.Title, ClassificationRegistry.ScenarioTitle, snapshot))
                .Where(c => c != null);
        }

        private ClassificationSpan CreateClassification(int sourceLine, string text, IClassificationType classificationType, ITextSnapshot snapshot)
        {
            int descriptionStartPosition = snapshot.GetLineFromLineNumber(sourceLine).Start.Position;
            int lineNumber = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Count() + sourceLine;
            if (snapshot.LineCount <= lineNumber)
                return null;
            int descriptionEndPosition = snapshot.GetLineFromLineNumber(lineNumber).Start.Position;
            var descriptionSpan = new Span(descriptionStartPosition, descriptionEndPosition - descriptionStartPosition);

            return new ClassificationSpan(new SnapshotSpan(snapshot, descriptionSpan), classificationType);
        }
    }
}