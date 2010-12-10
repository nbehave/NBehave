using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class StepClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Step;
        }

        public override void RegisterClassificationDefinitions()
        {
            Register(@event => GetKeywordSpan(@event));
            Register(parserEvent => GetPlaceHolders(parserEvent));
        }

        private IEnumerable<ClassificationSpan> GetPlaceHolders(ParserEvent parserEvent)
        {
            ITextSnapshotLine lineFromLineNumber = parserEvent.Snapshot.GetLineFromLineNumber(parserEvent.Line - 1);
            Match match = new Regex(@"((?:[\w\s]*)(?<placeholder>[\[|<]\w*[\]|>])(?:[\w\s]*))*").Match(lineFromLineNumber.GetText());

            if(match.Groups["placeholder"].Captures.Count > 0)
            {
                foreach (Capture capture in match.Groups["placeholder"].Captures)
                {
                    yield return new ClassificationSpan(new SnapshotSpan(parserEvent.Snapshot,
                                                                         new Span(lineFromLineNumber.Start.Position + capture.Index, capture.Length)),
                                                        ClassificationRegistry.Placeholder);
                }
            }
        }
    }
}