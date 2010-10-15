using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    public abstract class GherkinClassifierBase : IGherkinClassifier
    {
        [Import]
        public GherkinFileEditorClassifications ClassificationRegistry { get; set; }

        protected ClassificationSpan GetTitleSpan(ITextSnapshotLine textSnapshotLine, ITextSnapshot snapshot)
        {
            string lineFromLineNumber = textSnapshotLine.GetText();

            var titleMatches = new Regex(":").Match(lineFromLineNumber);
            var titleSpan = new Span(textSnapshotLine.Start.Position + titleMatches.Captures[0].Index + 1, lineFromLineNumber.Substring(titleMatches.Captures[0].Index + 1).Length);
            return new ClassificationSpan(new SnapshotSpan(snapshot, titleSpan), ClassificationRegistry.FeatureTitle);
        }

        protected ClassificationSpan GetKeywordSpan(ITextSnapshotLine textSnapshotLine, string keyword, ITextSnapshot snapshot)
        {
            string lineFromLineNumber = textSnapshotLine.GetText();

            var keywordMatches = new Regex("^\\s*" + keyword).Match(lineFromLineNumber);
            var keywordSpan = new Span(textSnapshotLine.Start.Position + keywordMatches.Captures[0].Index, keywordMatches.Captures[0].Length);

            return new ClassificationSpan(new SnapshotSpan(snapshot, keywordSpan), ClassificationRegistry.Keyword);
        }

        public abstract bool CanClassify(ParserEvent parserEvent);
        public abstract IList<ClassificationSpan> Classify(ParserEvent event3);
    }
}