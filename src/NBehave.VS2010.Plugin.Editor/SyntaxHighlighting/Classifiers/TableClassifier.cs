using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class TableClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Table;
        }

        public override void RegisterClassificationDefinitions()
        {
            Register(parserEvent => GetColumns(parserEvent));
        }

        private IEnumerable<ClassificationSpan> GetColumns(ParserEvent parserEvent)
        {
            ITextSnapshotLine lineFromLineNumber = parserEvent.Snapshot.GetLineFromLineNumber(parserEvent.Line - 1);

            MatchCollection match = new Regex(@"(?:\|?)(?<column>(\s*[\w]+\s*))(?:\|)").Matches(lineFromLineNumber.GetText());

            IEnumerable<ClassificationSpan> tableHeaderClassifications = 
                from Match m in match
                    let column = m.Groups["column"].Captures[0]
                    select ToClassificationSpan(parserEvent, lineFromLineNumber, column);

            return tableHeaderClassifications;
        }

        private ClassificationSpan ToClassificationSpan(ParserEvent parserEvent, ITextSnapshotLine lineFromLineNumber, Capture column)
        {
            var start = new Span(lineFromLineNumber.Start.Position + column.Index, column.Length);
            var snapshotSpan = new SnapshotSpan(parserEvent.Snapshot, start);
            return new ClassificationSpan(snapshotSpan, ClassificationRegistry.TableHeader);
        }
    }
}