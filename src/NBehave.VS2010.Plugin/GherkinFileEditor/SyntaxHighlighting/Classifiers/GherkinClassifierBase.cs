using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    public abstract class GherkinClassifierBase : IGherkinClassifier
    {
        private List<Func<ParserEvent, ClassificationSpan>> definitions = new List<Func<ParserEvent, ClassificationSpan>>();

        [Import]
        public GherkinFileEditorClassifications ClassificationRegistry { get; set; }

        protected GherkinClassifierBase()
        {
            definitions = new List<Func<ParserEvent, ClassificationSpan>>();
            RegisterClassificationDefinitions();
        }

        protected ClassificationSpan GetTitleSpan(ParserEvent parserEvent, IClassificationType classificationType)
        {
            ITextSnapshotLine textSnapshotLine = parserEvent.Snapshot.GetLineFromLineNumber(parserEvent.Line - 1);

            string lineFromLineNumber = textSnapshotLine.GetText();

            var titleMatches = new Regex(":").Match(lineFromLineNumber);
            var titleSpan = new Span(textSnapshotLine.Start.Position + titleMatches.Captures[0].Index + 1, lineFromLineNumber.Substring(titleMatches.Captures[0].Index + 1).Length);
            return new ClassificationSpan(new SnapshotSpan(parserEvent.Snapshot, titleSpan), classificationType);
        }

        protected ClassificationSpan GetKeywordSpan(ParserEvent parserEvent)
        {
            ITextSnapshotLine textSnapshotLine = parserEvent.Snapshot.GetLineFromLineNumber(parserEvent.Line - 1);

            string lineFromLineNumber = textSnapshotLine.GetText();

            var keywordMatches = new Regex("^\\s*" + parserEvent.Keyword).Match(lineFromLineNumber);
            var keywordSpan = new Span(textSnapshotLine.Start.Position + keywordMatches.Captures[0].Index, keywordMatches.Captures[0].Length);

            return new ClassificationSpan(new SnapshotSpan(parserEvent.Snapshot, keywordSpan), ClassificationRegistry.Keyword);
        }

        public abstract bool CanClassify(ParserEvent parserEvent);

        public IList<ClassificationSpan> Classify(ParserEvent parserEvent)
        {
            List<ClassificationSpan> spans = new List<ClassificationSpan>();

            try
            {
                spans.AddRange(definitions.Select(definition => definition.Invoke(parserEvent)));
            }
            catch (Exception) { }

            return spans;
        }

        public abstract void RegisterClassificationDefinitions();

        public void Register(Func<ParserEvent, ClassificationSpan> definition)
        {
            definitions.Add(definition);
        }
    }
}